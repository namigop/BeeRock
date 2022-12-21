namespace BeeRock.Core.Entities.ObjectBuilder;

internal interface ITypeBuilder {
    ValueTuple<bool, object> Build(Type type, int counter);
}