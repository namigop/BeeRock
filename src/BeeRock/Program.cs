using Avalonia;
using Avalonia.ReactiveUI;
using BeeRock.Core.Entities;
using BeeRock.Models;

namespace BeeRock;

internal class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        try {
            Global.Trace = new ConsoleIntercept();
            Console.SetOut(Global.Trace);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exc) {
            File.WriteAllText("Unhandled.error.txt", exc.ToString());
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}
