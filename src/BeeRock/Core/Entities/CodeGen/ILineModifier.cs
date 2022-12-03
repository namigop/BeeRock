namespace BeeRock.Core.Utils;

interface ILineModifier {
    bool CanModify(string line, int lineNumber);
    string Modify();
}
