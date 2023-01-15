using LanguageExt.Common;

namespace BeeRock.Core.Utils;

public static class RequiresExtension {

    public static Result<T> Bind<T>(this Result<T> r, Func<Result<T>> nextFunc) {
        if (r.IsFaulted)
            return r;

        return nextFunc();
    }

    public static Result<S> Map<T, S>(this Result<T> r, Func<Result<S>> nextFunc) {
        var d = r.Match(o => null, exc => exc);
        if (d != null)
            return new Result<S>(d);
        return nextFunc();
    }
}