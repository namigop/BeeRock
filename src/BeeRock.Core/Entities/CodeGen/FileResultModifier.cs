namespace BeeRock.Core.Entities.CodeGen;

public class FileResultModifier : ILineModifier {
    private const string OldText = "<FileResult>";
    private const string NewText = "<Microsoft.AspNetCore.Mvc.FileContentResult>";

    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Contains(OldText);
    }


    public string Modify() {
        return _currentLine.Replace(OldText, NewText);
    }
}
