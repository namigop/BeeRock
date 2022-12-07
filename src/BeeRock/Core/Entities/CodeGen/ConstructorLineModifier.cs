using System.Text.RegularExpressions;

namespace BeeRock.Core.Entities.CodeGen;

public class ConstructorLineModifier : ILineModifier {
    private const string CtrRegex = @"public\s+(?<ClassName>.*)Controller\(I.*implementation\)";

    private string _currentLine;
    private int _lineNumber;

    public string ClassName { get; private set; }

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        var m = Regex.Match(currentLine, CtrRegex);
        if (m.Success) {
            ClassName = m.Groups["ClassName"].Value;
            return true;
        }

        return false;
    }

    public string Modify() {
        //We dont need the constructor that takes in an IController implementation because the method will of the
        //controller class will be later on modified.

        int d =-1;
        var newClassName = this.ClassName;
        if (this.ClassName.Length > 0 && int.TryParse(ClassName[0].ToString(), out d)) {
            newClassName = $"C{ClassName}";
        }
        var newConstructor = $"public {newClassName}Controller()";
        var oldConstructor = $"public {ClassName}Controller(I{ClassName}Controller implementation)";
        return _currentLine.Replace(oldConstructor, newConstructor);
    }
}
