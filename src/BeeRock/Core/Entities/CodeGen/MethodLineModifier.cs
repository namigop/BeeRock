using System.Text.RegularExpressions;

namespace BeeRock.Core.Utils;

public class MethodLineModifier : ILineModifier
{
    private  string _currentLine;
    private  int _lineNumber;
    private  string _methodName;
    private const string MethodRegex = @"\s+System.Threading.Tasks.Task.*\s(?<MethodName>\w+)\(.*\)";

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, MethodRegex);
        if (m.Success) {

            _methodName = m.Groups["MethodName"].Value;
            return true;
        }

        return false;
    }

    public string MethodName => _methodName;

    public string Modify() {
        //Sometimes NSwag generates method names line 1Async, 2Async. These names start with a number and will cause
        //the compilation to fail. Also, sometimes duplicate method names are generated. So we fix up the method names by
        //making it unique.

        return _currentLine.Replace($"{_methodName}", $"M{_methodName}{_lineNumber}");
    }
}
