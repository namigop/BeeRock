namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarRmq {
    public void Publish(string hostName, string queue, string exchange, string routingKey, string message) {
        RmqClient.Publish(hostName, queue, exchange, routingKey, message);
    }
}