namespace BeeRock.Models;

interface ITypeBuilder {
    ValueTuple<bool, object> Build(Type type, int counter);
}
