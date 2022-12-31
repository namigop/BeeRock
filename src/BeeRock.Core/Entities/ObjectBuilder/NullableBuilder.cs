namespace BeeRock.Core.Entities.ObjectBuilder;

public class NullableBuilder : ITypeBuilder {
    /// <summary>
    ///     Create an instance of a Nullable<T> type
    /// </summary>
    public (bool, object) Build(Type type, int counter) {
        if (type.FullName.StartsWith("System.Nullable")) {
            var itemType = type.GenericTypeArguments.First();
            var itemInstance = ObjectBuilder.CreateNewInstance(itemType, counter) ?? ObjectBuilder.Populate(Activator.CreateInstance(itemType));
            return (true, itemInstance);
        }

        return (false, null);
    }
}
