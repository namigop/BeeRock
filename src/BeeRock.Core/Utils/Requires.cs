using LanguageExt.Common;

namespace BeeRock.Core.Utils;

public static class Requires {

    public static void IsTrue(Func<bool> f, string name) {
        if (!f())
            throw new RequiresException($"Condition \"{name}\" returned false");
    }

    public static void IsTrue<T>(Func<T, bool> f, T arg, string name) {
        if (!f(arg))
            throw new RequiresException($"Condition \"{name}\" returned false");
    }

    public static Result<T> IsTrue2<T>(Func<bool> f, string name) {
        if (!f())
            return new Result<T>(new RequiresException($"Condition \"{name}\" returned false"));

        return new Result<T>(default(T));
    }

    public static void NotNull(object o, string name) {
        if (o == null)
            throw new RequiresException($"{name} cannot be null");
    }

    public static Result<T> NotNull2<T>(object o, string name) {
        if (o == null)
            return new Result<T>(new RequiresException($"{name} cannot be null"));

        return new Result<T>(default(T));
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

    public static Result<T> NotNullOrEmpty2<T>(string o, string name) {
        if (string.IsNullOrEmpty(o))
            return new Result<T>(new RequiresException($"{name} cannot be null or empty"));

        return new Result<T>(default(T));
    }

    public static Result<T> NotNullOrEmpty2<C, T>(ICollection<C> o, string name) {
        return NotNull2<T>(o, name)
            .Bind(() => {
                if (o.Count == 0)
                    return new Result<T>(new RequiresException($"Collection of type {o.GetType().Name} {name} cannot be null or empty."));

                return new Result<T>(default(T));
            });
    }
}