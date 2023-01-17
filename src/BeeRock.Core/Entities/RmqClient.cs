using System;
using RabbitMQ.Client;
using System.Text;

namespace BeeRock.Core.Entities;

public static class RmqClient {
    public static void Publish(string hostName, string queue, string exchange, string routingKey, string message) {
        var factory = new ConnectionFactory() { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange, routingKey, basicProperties: null, body: body);
    }
}
