using System.Reflection;

namespace BeeRock.Core.Utils;

public static class RedirectCalls {
    private static readonly MethodInfo method =
        Assembly.GetEntryAssembly()
            .GetType("BeeRock.Core.Utils.RequestHandler")
            .GetMethod("Handle");

    public static Dictionary<string, object> CreateParameter(string[] keys, object[] values) {
        var dict = new Dictionary<string, object>();
        for (var i = 0; i < keys.Length; i++) dict.Add(keys[i], values[i]);

        return dict;
    }

    public static string HandleResponse(string methodName, Dictionary<string, object> parameters) {
        var p = new List<object>();
        p.Add(methodName);
        p.Add(parameters);
        return method.Invoke(null, p.ToArray()).ToString();
    }
}
