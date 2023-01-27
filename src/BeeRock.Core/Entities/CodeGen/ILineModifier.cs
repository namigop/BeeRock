namespace BeeRock.Core.Entities.CodeGen;

internal interface ILineModifier {
    bool CanModify(string line, int lineNumber);

    string Modify();
}