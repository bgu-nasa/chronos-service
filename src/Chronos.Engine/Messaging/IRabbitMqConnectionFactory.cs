using RabbitMQ.Client;

namespace Chronos.Engine.Messaging;

public interface IRabbitMqConnectionFactory
{
    IConnection CreateConnection();
    IModel CreateChannel();
}
