using System.Text;
using BeeRock.Core.Utils;
using RabbitMQ.Client;

namespace BeeRock.Core.Entities;

public static class RmqClient {
    public static void Publish(string uri, string queue, string exchange, string routingKey, string message) {
        if (Uri.TryCreate(uri, UriKind.Absolute, out var uri2)) {
            var factory = new ConnectionFactory { Uri = uri2 };
            Publish(factory, queue, exchange, routingKey, message);
        }
        else {
            C.Error($"Failed to create an RMQ connection. \"{uri}\" is invalid.");
        }
    }

    private static void Publish(ConnectionFactory factory, string queue, string exchange, string routingKey, string message) {
        try {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueBind(queue, exchange, routingKey);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange, routingKey, null, body);
        }
        catch (Exception exc) {
            C.Error((exc.ToString()));
        }
    }
}
