using System.Text.RegularExpressions;

namespace BeeRock.Core.Entities.CodeGen;

/// <summary>
///     Prefixes "C" to the controller name to ensure the classname is always valid
/// </summary>
public class ControllerClassNameModifier : ILineModifier {

    // public partial class 1Controller : Microsoft.AspNetCore.Mvc.ControllerBase
    private const string CtrRegex = @"public\s+partial\s+class\s+(?<ClassName>.*Controller).*Microsoft.AspNetCore.Mvc.ControllerBase";

    private string _className;

    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, CtrRegex);
        if (m.Success) {
            _className = m.Groups["ClassName"].Value;
            var d = -1;
            if (int.TryParse(_className[0].ToString(), out d)) return true;
        }

        return false;
    }

    public string Modify() {
        //Change the class name to start with a letter so that it is always valid because sometimes NSwag generates classnames like 1Controller
        var oldClassName = $" {_className} :";
        var newClassName = $" C{_className} :";
        return _currentLine.Replace(oldClassName, newClassName);
    }
}