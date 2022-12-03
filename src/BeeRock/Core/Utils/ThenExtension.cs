namespace BeeRock.API;

public static class ThenExtension {
    public static B Then<A, B>(this A x, Func<A,B> func ) {
        return func(x);
    }
    public static void Void<A>(this A x, Action<A> func ) {
        func(x);
    }
}