using System.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace BeeRock.Core.Entities;

public class ScriptedJson {
    /// <summary>
    ///     Recursively evaluate one or more python blocks marked by  <<..>> within a single line
    ///     The result of the evaluation is inserted back into the json text
    /// </summary>
    public static string Evaluate(string responseBodyJson, Dictionary<string, object> variables) {
        Requires.NotNullOrEmpty(responseBodyJson, nameof(responseBodyJson));

        static string EvaluateLine(string line, Dictionary<string, object> vars) {
            if (line.Length > 4 && line.Contains(Scripting.BeginMarker) && line.Contains(Scripting.EndMarker)) {
                //an expression is between << >>, hence the +2 or -2 in the substrings
                var start = line.IndexOf(Scripting.BeginMarker, StringComparison.Ordinal);
                var end = line.IndexOf(Scripting.EndMarker, StringComparison.Ordinal);
                var expression = line.Substring(start + 2, end - start - 2);
                var ret = PyEngine.Evaluate(expression, vars);
                line = line.Substring(0, start) + $"{ret}" + line.Substring(end + 2);
                return EvaluateLine(line, vars);
            }

            return line;
        }

        var newJson = new StringBuilder();
        using var reader = new StringReader(responseBodyJson);
        while (reader.ReadLine() is { } line) {
            if (line.TrimStart().StartsWith("//")) //ignore comments in the json text
                continue;

            newJson.AppendLine(EvaluateLine(line, variables));
        }

        return newJson.ToString();
    }
}
