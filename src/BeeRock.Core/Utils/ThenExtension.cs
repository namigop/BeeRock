namespace BeeRock.Core.Utils;

public static class ThenExtension {

    public static TB Then<TA, TB>(this TA x, Func<TA, TB> func) {
        return func(x);
    }

    public static void Void<TA>(this TA x, Action<TA> func) {
        func(x);
    }
}