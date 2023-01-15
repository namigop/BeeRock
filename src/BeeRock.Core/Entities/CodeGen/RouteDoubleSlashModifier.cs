namespace BeeRock.Core.Entities.CodeGen;

public class RouteDoubleSlashModifier : ILineModifier {
    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Contains("Microsoft.AspNetCore.Mvc.Route(") && currentLine.Contains("//");
    }

    public string Modify() {
        return _currentLine.Replace("//", "/");
    }
}