using System.Reflection;

namespace BeeRock.Core.Utils;

public static class RedirectCalls {
    private static readonly MethodInfo method =
        Assembly.GetEntryAssembly()
            .GetType("BeeRock.Core.Utils.RequestHandler")
            .GetMethod("Handle");

    public static string HandleResponse(string methodName, object[] parameters) {
        var p = new List<object>(parameters);
        p.Insert(0, methodName);
        method.Invoke(null, p.ToArray());

        return "";
    }
}