namespace BeeRock.Core.Entities.CodeGen;

/// <summary>
///     Replace IDictionary with concrete Dictionary<T>
/// </summary>
public class ReadOnlyDictionaryModifier : ILineModifier {
    private const string NewText = "System.Collections.Generic.Dictionary";
    private const string OldText = "System.Collections.Generic.IReadOnlyDictionary";
    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Contains(OldText);
    }

    public string Modify() {
        //We need something that can deserialize to concrete type
        return _currentLine.Replace(OldText, NewText);
    }
}
