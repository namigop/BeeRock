namespace BeeRock.Models;

public static class ObjectBuilder {
    private static List<ITypeBuilder> _builders = new() {
        new SystemTypeBuilder(),
        new EnumBuilder(),
        new NullableBuilder(),
        new ListBuilder(),
        new DictBuilder(),
        new ClassBuilder() //should be the last one
    };

    public static object CreateNewInstance(Type type, int counter) {
        foreach (var b in _builders) {
            var (ok, instance) = b.Build(type, counter);
            if (ok)
                return instance;
        }

        return null;
    }

    private static int maxdepth = 10;

    internal static object Populate(object instance, int counter = 0) {
        if (counter > maxdepth)
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
