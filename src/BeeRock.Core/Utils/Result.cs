namespace BeeRock.Core.Utils;

public static class Result {
    public static R<T> Create<T>(T item) {
        return new R<T>(item);
    }

    public static R<T> Error<T>(Exception exc) {
        return new R<T>(exc);
    }

    public class R<T> {
        public R(T value) {
            Value = value;
        }

        public R(Exception error) {
            Error = error;
        }

        public bool IsFailed => Error != null;
        public T Value { get; }
        public Exception Error { get; }
    }
}
