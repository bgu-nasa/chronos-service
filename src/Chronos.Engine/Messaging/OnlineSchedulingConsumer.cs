using System.Text;
using System.Text.Json;
using Chronos.Domain.Schedule.Messages;
using Chronos.Engine.Configuration;
using Chronos.Engine.Matching;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chronos.Engine.Messaging;

public class OnlineSchedulingConsumer : BackgroundService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly MatchingOrchestrator _orchestrator;
    private readonly IMessagePublisher _messagePublisher;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<OnlineSchedulingConsumer> _logger;
    private IModel? _channel;

    public OnlineSchedulingConsumer(
        IRabbitMqConnectionFactory connectionFactory,
        MatchingOrchestrator orchestrator,
        IMessagePublisher messagePublisher,
        IOptions<RabbitMqOptions> options,
        ILogger<OnlineSchedulingConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _orchestrator = orchestrator;
        _messagePublisher = messagePublisher;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Online Scheduling Consumer starting...");

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
                        "Received online scheduling request. DeliveryTag: {DeliveryTag}",
                        ea.DeliveryTag);

                    var request = JsonSerializer.Deserialize<HandleConstraintChangeRequest>(message);

                    if (request == null)
                    {
                        _logger.LogError("Failed to deserialize HandleConstraintChangeRequest");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    // Execute online scheduling
                    var result = await _orchestrator.ExecuteAsync(
                        request,
                        SchedulingMode.Online,
                        stoppingToken);

                    // Publish result
                    await _messagePublisher.PublishAsync(
                        result,
                        result.Success ? "result.success" : "result.failed");

                    _channel.BasicAck(ea.DeliveryTag, false);

                    _logger.LogInformation(
                        "Online scheduling completed. Success: {Success}, Modified: {Modified}",
                        result.Success,
                        result.AssignmentsModified);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing online scheduling request");
                    _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue
                }
            };

            _channel.BasicConsume(
                queue: _options.OnlineQueueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation(
                "Online Scheduling Consumer started, listening to queue: {QueueName}",
                _options.OnlineQueueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Online Scheduling Consumer error");
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
