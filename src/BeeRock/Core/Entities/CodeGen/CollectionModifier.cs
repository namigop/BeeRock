namespace BeeRock.Core.Utils;

public class CollectionModifier : ILineModifier {

    private const string ICollectionText = $"System.Collections.Generic.ICollection";
    private const string IEnumerableText = $"System.Collections.Generic.IEnumerable";
    private const string ListText = $"System.Collections.Generic.List";

    private  string _currentLine;
    private  int _lineNumber;

    public bool CanModify(string currentLine, int lineNumber) {
        _currentLine = currentLine;
        _lineNumber = lineNumber;
        return currentLine.Contains(ICollectionText) || currentLine.Contains(IEnumerableText);;
    }


    public string Modify() {
        //We need something that can deserialize to concrete type
        return  _currentLine.Replace(ICollectionText, ListText).Replace(IEnumerableText, ListText);
    }
}
