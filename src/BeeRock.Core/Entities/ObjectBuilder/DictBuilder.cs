namespace BeeRock.Core.Entities.ObjectBuilder;

public class DictBuilder : ITypeBuilder {
    /// <summary>
    ///     Create an instance of a Dictionary<K, V> type
    /// </summary>
    public (bool, object) Build(Type type, int counter) {
        if (type.FullName.StartsWith("System.Collections.Generic.Dictionary")) {
            var dictionary = Activator.CreateInstance(type);
            var keyType = type.GenericTypeArguments[0];
            var valType = type.GenericTypeArguments[1];

            var keyInstance = ObjectBuilder.CreateNewInstance(keyType, counter) ?? ObjectBuilder.Populate(Activator.CreateInstance(keyType), counter);
            var valueInstance = ObjectBuilder.CreateNewInstance(valType, counter) ?? ObjectBuilder.Populate(Activator.CreateInstance(valType), counter);

            var m = type.GetMethod("Add");
            m.Invoke(dictionary, new[] { keyInstance, valueInstance });
            return (true, dictionary);
        }

        return (false, null);
    }
}