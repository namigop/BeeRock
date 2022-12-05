namespace BeeRock.Core.Entities.ObjectBuilder;

public class SystemTypeBuilder : ITypeBuilder {
    public (bool, object) Build(Type type, int counter) {
        if (type == typeof(string))
            return (true, "string");
        if (type == typeof(int))
            return (true, 0);
        if (type == typeof(long))
            return (true, 0L);
        if (type == typeof(uint))
            return (true, 0U);
        if (type == typeof(ulong))
            return (true, 0UL);
        if (type == typeof(short))
            return (true, (short)0);
        if (type == typeof(double))
            return (true, 0.0);
        if (type == typeof(bool))
            return (true, false);
        if (type == typeof(float))
            return (true, 0.0f);
        if (type == typeof(decimal))
            return (true, 0M);
        if (type == typeof(DateTime))
            return (true, DateTime.Now);

        return (false, null);
    }
}
