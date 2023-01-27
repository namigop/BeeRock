using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities.ObjectBuilder;

public class ClassBuilder : ITypeBuilder {
    /// <summary>
    ///     Create an instance of a class type
    /// </summary>
    public (bool, object) Build(Type type, int counter) {
        if (type.IsClass) {
            var instance = Activator.CreateInstance(type)
                .Then(i => ObjectBuilder.Populate(i, counter));
            return (true, instance);
        }

        return (false, null);
    }
}