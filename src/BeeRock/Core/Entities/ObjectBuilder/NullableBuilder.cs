namespace BeeRock.Models;

public class NullableBuilder : ITypeBuilder{

    public (bool, object) Build(Type type, int counter) {
        if (type.FullName.StartsWith("System.Nullable")) {
            var itemType = type.GenericTypeArguments.First();
            var itemInstance = ObjectBuilder.CreateNewInstance(itemType, counter) ?? ObjectBuilder.Populate(Activator.CreateInstance(itemType));
            return(true, itemInstance);
        }
        return (false, null);
    }
}
