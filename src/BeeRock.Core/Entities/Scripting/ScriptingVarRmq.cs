// ReSharper disable UnusedMember.Global
namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarRmq {
    public void Publish(string uri, string queue, string exchange, string routingKey, string message) {
        RmqClient.Publish(uri, queue, exchange, routingKey, message);
    }
}
