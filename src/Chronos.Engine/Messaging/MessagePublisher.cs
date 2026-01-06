using System.Text;
using System.Text.Json;
using Chronos.Engine.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Chronos.Engine.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(
        IRabbitMqConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<MessagePublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        try
        {
            using var channel = _connectionFactory.CreateChannel();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogDebug(
                "Published message of type {MessageType} to exchange {Exchange} with routing key {RoutingKey}",
                typeof(T).Name,
                _options.ExchangeName,
                routingKey);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }
}
