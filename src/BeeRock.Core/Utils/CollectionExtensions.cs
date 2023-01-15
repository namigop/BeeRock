namespace BeeRock.Core.Utils;

public static class CollectionExtensions {

    public static bool IsEmpty<T>(this IEnumerable<T> source) {
        Requires.NotNull(source, nameof(source));
        return !source.Any();
    }
}