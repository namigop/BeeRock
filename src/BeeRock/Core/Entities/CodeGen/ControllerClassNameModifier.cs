using System.Text.RegularExpressions;

namespace BeeRock.Core.Entities.CodeGen;

public class ControllerClassNameModifier : ILineModifier {
    // public partial class 1Controller : Microsoft.AspNetCore.Mvc.ControllerBase
    private const string CtrRegex = @"public\s+partial\s+class\s+(?<ClassName>.*Controller).*Microsoft.AspNetCore.Mvc.ControllerBase";

    private string _currentLine;
    private int _lineNumber;

    public string ClassName { get; private set; }

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, CtrRegex);
        if (m.Success) {
            ClassName = m.Groups["ClassName"].Value;
            var d = -1;
            if (int.TryParse(ClassName[0].ToString(), out d)) return true;
        }

        return false;
    }

    public string Modify() {
        //We dont need the constructor that takes in an IController implementation because the method will of the
        //controller class will be later on modified.

        var oldClassName = $" {ClassName} :";
        var newClassName = $" C{ClassName} :";
        return _currentLine.Replace(oldClassName, newClassName);
    }
}