using Newtonsoft.Json;

namespace BeeRock.Core.Entities.ObjectBuilder;

public static class ObjectBuilder {
    private const int MaxDepth = 10;

    private static readonly List<ITypeBuilder> Builders = new() {
        new SystemTypeBuilder(),
        new EnumBuilder(),
        new NullableBuilder(),
        new ListBuilder(),
        new DictBuilder(),
        new ClassBuilder() //should be the last one
    };

    public static string CreateNewInstanceAsJson(Type type, int counter) {
        var instance = CreateNewInstance(type, counter);
        try {
            var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            return json;
        }
        catch {
            return "{}";
        }
    }


    public static object CreateNewInstance(Type type, int counter) {
        static object GetDefaultFor(Type thisType) {
            var method = typeof(ObjectBuilder).GetMethod(nameof(ObjectBuilder.GetDefault));
            var generic = method.MakeGenericMethod(thisType);
            return generic.Invoke(null, null);
        }
        try {
            foreach (var b in Builders) {
                var (ok, instance) = b.Build(type, counter);
                if (ok)
                    return instance;
            }

            return GetDefaultFor(type);
        }
        catch {
            //for any errors, just use the default instances
            return GetDefaultFor(type);
        }
    }

    public static T GetDefault<T>() {
        return default(T);
    }

    internal static object Populate(object instance, int counter = 0) {
        if (counter > MaxDepth)
            return null;

        counter += 1;
        if (instance == null)
            return null;

        var instanceType = instance.GetType();

        try {
            var props = instanceType.GetProperties().Where(p => p.CanWrite);
            foreach (var prop in props) {
                var val = CreateNewInstance(prop.PropertyType, counter);
                prop.SetValue(instance, val);
            }

            return instance;
        }
        catch {
            return null;
        }
    }
}
