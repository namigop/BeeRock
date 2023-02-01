using System.Text.RegularExpressions;

namespace BeeRock.Core.Entities.CodeGen;

/// <summary>
///     Prefixes the methodName with "M" and adds the line number to ensure that the method
///     name is always unique
/// </summary>
public class MethodLineColonModifier : ILineModifier {
    private const string MethodRegex = @"\s+System.Threading.Tasks.Task.*\s(?<MethodName>[:]?\w+)\(.*\)";
    private string _currentLine;
    private int _lineNumber;

    public string MethodName { get; private set; }

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        if (currentLine.Contains("return :"))
            return true;

        var m = Regex.Match(currentLine, MethodRegex);
        if (m.Success) {
            MethodName = m.Groups["MethodName"].Value;
            return true;
        }

        return false;
    }

    public string Modify() {
        return _currentLine
            .Replace(":", ""); //colons are fine in the URL but not in method names
    }
}
