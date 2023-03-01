// ReSharper disable UnusedMember.Global
namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarRmq {
    public void Publish(string hostName, string queue, string exchange, string routingKey, string message, bool durable=true, bool exclusive=false, bool autoDelete=false) {
        RmqClient.Publish(hostName, queue, exchange, routingKey, message, durable, exclusive, autoDelete);
    }
}
