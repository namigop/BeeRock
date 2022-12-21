using System.Text;

namespace BeeRock.Core.Entities.CodeGen;

internal interface ICodeModifier {
    StringBuilder Modify();
}