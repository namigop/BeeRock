namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
public static class RequestHandler {
    public static string Handle(string methodName, Dictionary<string, object> foo) {
        Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        var m = Global.CurrentServices
            .SelectMany(c => c.Methods)
            .First(t => t.Method.MethodName == methodName);
        
        return ScriptedJson.Evaluate(m.ResponseText, foo);
    }
}
