using System.Text.RegularExpressions;

namespace BeeRock.Core.Entities.CodeGen;

/// <summary>
///     Prefixes the methodName with "M" and adds the line number to ensure that the method
///     name is always unique
/// </summary>
public class MethodLineModifier : ILineModifier {
    private const string MethodRegex = @"\s+System.Threading.Tasks.Task.*\s(?<MethodName>[:]?\w+)\(.*\)";
    private string _currentLine;
    private int _lineNumber;

    public string MethodName { get; private set; }

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, MethodRegex);
        if (m.Success) {
            MethodName = m.Groups["MethodName"].Value;
            return true;
        }

        return false;
    }

    public string Modify() {
        //Sometimes NSwag generates method names line 1Async, 2Async. These names start with a number and will cause
        //the compilation to fail. Also, sometimes duplicate method names are generated. So we fix up the method names by
        //making it unique.

        return _currentLine
            .Replace($" {MethodName}(", $" M{MethodName}_{_lineNumber}(")
            .Replace(":", ""); //colons are fine in the URL but not in method names
    }
}
