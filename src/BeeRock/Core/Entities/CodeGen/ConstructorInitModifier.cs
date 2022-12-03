namespace BeeRock.Core.Utils;

public class ConstructorInitModifier : ILineModifier {
    private const string CtrLine = @"_implementation = implementation;";

    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Trim() == CtrLine;
    }


    public string Modify() {
        //We dont need the constructor that takes in an IController implementation because the method will of the
        //controller class will be later on modified.
        return _currentLine.Replace(CtrLine, "");
    }
}
