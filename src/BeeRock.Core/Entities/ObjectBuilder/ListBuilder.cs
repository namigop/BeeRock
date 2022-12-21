namespace BeeRock.Core.Entities.ObjectBuilder;

public class ListBuilder : ITypeBuilder {
    public (bool, object) Build(Type type, int counter) {
        if (type.FullName.StartsWith("System.Collections.Generic.List")) {
            var listInstance = Activator.CreateInstance(type);
            var itemType = type.GenericTypeArguments.First();
            var itemInstance = ObjectBuilder.CreateNewInstance(itemType, counter) ??
                               ObjectBuilder.Populate(Activator.CreateInstance(itemType), counter);
            var addM = type.GetMethod("Add");
            addM.Invoke(listInstance, new[] { itemInstance });
            return (true, listInstance);
        }

        return (false, null);
    }
}