using System.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptedJson {
    /// <summary>
    ///     Recursively evaluate one or more python blocks marked by  <<..>> within a single line
    ///     The result of the evaluation is inserted back into the json text
    /// </summary>
    public static string Evaluate(string responseBodyJson, string swaggerUrl, string serverMethod, Dictionary<string, object> variables) {
        Requires.NotNullOrEmpty(responseBodyJson, nameof(responseBodyJson));

        static (bool, string) EvaluateLine(string line, string swagUrl, string serverMethod2, Dictionary<string, object> vars) {
            //evaluate 1-liner expression
            if (Expression.ContainsExpression(line)) {
                //an expression is between << >>, hence the +2 or -2 in the substrings
                var start = line.IndexOf(Expression.BeginMarker, StringComparison.Ordinal);
                var end = line.IndexOf(Expression.EndMarker, StringComparison.Ordinal);
                var expression = line.Substring(start + 2, end - start - 2);
                var ret = PyEngine.Evaluate(expression, swagUrl, serverMethod2, vars);
                line = line.Substring(0, start) + $"{ret}" + line.Substring(end + 2);
                return EvaluateLine(line, swagUrl, serverMethod2, vars);
            }

            //multi-line but without the closing >>
            if (line.Length >= 2 && line.Contains(Expression.BeginMarker)) return (false, "");

            return (true, line);
        }

        var newJson = new StringBuilder();
        using var reader = new StringReader(responseBodyJson);
        var temp = new StringBuilder();
        while (reader.ReadLine() is { } line) {
            if (line.TrimStart().StartsWith("//")) //ignore comments in the json text
                continue;

            temp.Append(line);
            var (evaluated, updatedLine) = EvaluateLine(temp.ToString(), swaggerUrl, serverMethod, variables);
            if (evaluated) {
                temp.Clear();
                newJson.AppendLine(updatedLine);
            }
            else {
                temp.AppendLine();
            }
        }

        return newJson.ToString();
    }
}
