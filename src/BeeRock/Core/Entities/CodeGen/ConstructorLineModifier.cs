using System.Text.RegularExpressions;

namespace BeeRock.Core.Utils;

public class ConstructorLineModifier : ILineModifier {
    private const string CtrRegex = @"public\s+(?<ClassName>.*)Controller\(I.*implementation\)";

    private  string _currentLine;
    private  int _lineNumber;
    private  string _className;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, CtrRegex);
        if (m.Success) {
            _className = m.Groups["ClassName"].Value;
            return true;
        }

        return false;
    }

    public string ClassName => _className;

    public string Modify() {
        //We dont need the constructor that takes in an IController implementation because the method will of the
        //controller class will be later on modified.

        var newConstructor = $"public {_className}Controller()";
        var oldConstructor = $"public {_className}Controller(I{_className}Controller implementation)";
        return _currentLine.Replace(oldConstructor, newConstructor);
    }
}
