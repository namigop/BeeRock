using System.Diagnostics;

namespace BeeRock.Core.Utils;

//Note: Writes to the console are redirected. See Program.Main()
public static class C {
    [Conditional("DEBUG")]
    public static void Debug(string msg) {
        Console.WriteLine($"DEBUG: {DateTime.Now} : {msg}");
    }

    public static void Error(string msg) {
        Console.WriteLine($"ERROR: {DateTime.Now} : {msg}");
    }

    public static void Info(string msg) {
        Console.WriteLine($"INFO: {DateTime.Now} : {msg}");
    }

    public static void Warn(string msg) {
        Console.WriteLine($"WARN: {DateTime.Now} : {msg}");
    }

    public static void NewLine() {
        Console.WriteLine();
    }
}
