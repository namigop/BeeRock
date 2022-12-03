namespace BeeRock.Core.Utils;

//This class will be called via Reflection by the generated REST API controller class
public static class RequestHandler {
    public static string Handle(string methodName, object[] foo) {
        Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        var m = Global.CurrentService.Methods.First(t => t.Method.MethodName == methodName);
        return m.ResponseText;
    }
}
