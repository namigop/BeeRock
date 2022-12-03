using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BeeRock.ViewModels;
using BeeRock.Views;

namespace BeeRock;

public class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var win = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            desktop.MainWindow = win;
            win.Init();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
