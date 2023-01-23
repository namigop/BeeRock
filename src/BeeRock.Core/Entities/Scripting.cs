namespace BeeRock.Core.Entities;

public static class Scripting {
    public const string BeginMarker = "<<";
    public const string EndMarker = ">>";

    /// <summary>
    ///     evaluate one-liner python expression.  Result can be of any type
    /// </summary>
    public static T Evaluate<T>(string line, string swaggerUrl, string serverMethod, Dictionary<string, object> vars) {
        if (line.Length > 4 && line.Contains(BeginMarker) && line.Contains(EndMarker)) {
            //an expression is between << >>, hence the +2 or -2 in the substrings
            var start = line.IndexOf(BeginMarker, StringComparison.Ordinal);
            var end = line.IndexOf(EndMarker, StringComparison.Ordinal);
            var expression = line.Substring(start + 2, end - start - 2);
            var ret = PyEngine.Evaluate(expression, swaggerUrl, serverMethod, vars);
            return (T)ret;
        }

        return default;
    }
}
