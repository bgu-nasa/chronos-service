using System.Text;
using System.Text.Json;
using Chronos.Domain.Schedule.Messages;
using Chronos.Engine.Configuration;
using Chronos.Engine.Matching;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chronos.Engine.Messaging;

public class BatchSchedulingConsumer : BackgroundService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly MatchingOrchestrator _orchestrator;
    private readonly IMessagePublisher _messagePublisher;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<BatchSchedulingConsumer> _logger;
    private IModel? _channel;

    public BatchSchedulingConsumer(
        IRabbitMqConnectionFactory connectionFactory,
        MatchingOrchestrator orchestrator,
        IMessagePublisher messagePublisher,
        IOptions<RabbitMqOptions> options,
        ILogger<BatchSchedulingConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _orchestrator = orchestrator;
        _messagePublisher = messagePublisher;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Batch Scheduling Consumer starting...");

        try
        {
            _channel = _connectionFactory.CreateChannel();
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation(
                        "Received batch scheduling request. DeliveryTag: {DeliveryTag}",
                        ea.DeliveryTag);

                    var request = JsonSerializer.Deserialize<SchedulePeriodRequest>(message);

                    if (request == null)
                    {
                        _logger.LogError("Failed to deserialize SchedulePeriodRequest");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    // Execute batch scheduling
                    var result = await _orchestrator.ExecuteAsync(
                        request,
                        SchedulingMode.Batch,
                        stoppingToken);

                    // Publish result
                    await _messagePublisher.PublishAsync(
                        result,
                        result.Success ? "result.success" : "result.failed");

                    _channel.BasicAck(ea.DeliveryTag, false);

                    _logger.LogInformation(
                        "Batch scheduling completed. Success: {Success}, Assignments: {Assignments}",
                        result.Success,
                        result.AssignmentsCreated);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch scheduling request");
                    _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue
                }
            };

            _channel.BasicConsume(
                queue: _options.BatchQueueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation(
                "Batch Scheduling Consumer started, listening to queue: {QueueName}",
                _options.BatchQueueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch Scheduling Consumer error");
            throw;
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}
