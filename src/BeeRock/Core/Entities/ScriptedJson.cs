using System.Text;

namespace BeeRock.Core.Entities;

public class ScriptedJson {
    private const string BeginMarker = "<<";
    private const string EndMarker = ">>";

    public static string Evaluate(string json, Dictionary<string, object> variables) {
        static string EvaluateLine(string line, Dictionary<string, object> vars) {
            if (line.Length > 4 && line.Contains(BeginMarker) && line.Contains(EndMarker)) {
                //an expression is between << >>, hence the +2 or -2 in the substrings
                var start = line.IndexOf(BeginMarker, StringComparison.Ordinal);
                var end = line.IndexOf(EndMarker, StringComparison.Ordinal);
                var expression = line.Substring(start + 2, end - start - 2);
                var ret = PyEngine.Evaluate(expression, vars);
                line = line.Substring(0, start) + $"{ret}" + line.Substring(end + 2);
                return EvaluateLine(line, vars);
            }

            return line;
        }

        var newJson = new StringBuilder();
        using var reader = new StringReader(json);
        while (reader.ReadLine() is { } line) {
            if (line.TrimStart().StartsWith("//")) //ignore comments in the json text
                continue;

            newJson.AppendLine(EvaluateLine(line, variables));
        }

        return newJson.ToString();
    }
}