namespace BeeRock.Core.Entities.CodeGen;

/// <summary>
///     Replaces ICollection and IEnumerable with concrete List<T> types
/// </summary>
public class CollectionModifier : ILineModifier {
    private const string CollectionText = "System.Collections.Generic.ICollection";
    private const string EnumerableText = "System.Collections.Generic.IEnumerable";
    private const string ListText = "System.Collections.Generic.List";

    private string _currentLine;
    private int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Contains(CollectionText) || currentLine.Contains(EnumerableText);
        ;
    }


    public string Modify() {
        //We need something that can deserialize to concrete type
        return _currentLine.Replace(CollectionText, ListText).Replace(EnumerableText, ListText);
    }
}
