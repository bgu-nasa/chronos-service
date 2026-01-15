namespace Chronos.Engine.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string routingKey)
        where T : class;
}
