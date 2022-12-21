using LanguageExt.Common;

namespace BeeRock.Core.Utils;

public static class RequiresExtension {
    public static Result<T> Bind<T>(this Result<T> r, Func<Result<T>> nextFunc) {
        if (r.IsFaulted)
            return r;

        return nextFunc();
    }
}

public static class Requires {
    public static void NotNull(object o, string name) {
        if (o == null)
            throw new RequiresException($"{name} cannot be null");
    }

    public static void NotNullOrEmpty(string o, string name) {
        if (string.IsNullOrEmpty(o))
            throw new RequiresException($"{name} cannot be null or empty");
    }

    public static Result<T> NotNullOrEmpty2<T>(string o, string name) {
        if (string.IsNullOrEmpty(o))
            return new Result<T>(new RequiresException($"{name} cannot be null or empty"));

        return new Result<T>(default(T));
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
        if (!f())
            throw new RequiresException($"Condition \"{name}\" returned false");
    }

    public static void IsTrue<T>(Func<T, bool> f, T arg, string name) {
        if (!f(arg))
            throw new RequiresException($"Condition \"{name}\" returned false");
    }
}
