using System.Text;
using RabbitMQ.Client;

namespace BeeRock.Core.Entities;

public static class RmqClient {
    public static void Publish(string hostName, string queue, string exchange, string routingKey, string message, bool durable=true, bool exclusive=false, bool autoDelete=false) {
        var factory = new ConnectionFactory { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue, durable, exclusive, autoDelete, null);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange, routingKey, null, body);
    }
}
