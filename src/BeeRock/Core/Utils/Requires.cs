namespace BeeRock.Core.Utils;

public static class Requires {
    public static void NotNull(object o, string name) {
        if (o == null)
            throw new RequiresException($"{name} cannot be null");
    }

    public static void NotNullOrEmpty(string o, string name) {
        if (string.IsNullOrEmpty(o))
            throw new RequiresException($"{name} cannot be null or empty");
    }

    public static void NotNullOrEmpty<T>(ICollection<T> o, string name) {
        NotNull(o, name);
        if (o.Count == 0)
            throw new RequiresException($"Collection of type {o.GetType().Name} {name} cannot be null or empty.");
    }

    public static void NotNullOrEmpty<T>(IEnumerable<T> o, string name) {
        NotNull(o, name);
        if (!o.Any())
            throw new RequiresException($"IEnumerable of type {o.GetType().Name} {name} cannot be null or empty.");
    }

    public static void IsTrue(Func<bool> f, string name) {
        if (f())
            throw new RequiresException($"Condition \"{name}\" returned false");
    }

    public static void IsTrue<T>(Func<T, bool> f, T arg, string name) {
        if (f(arg))
            throw new RequiresException($"Condition \"{name}\" returned false");
    }
}