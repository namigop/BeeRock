using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Adapters.UI.Views;
using BeeRock.Core.Utils;

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

    private void AboutMenuItem_OnClick(object sender, EventArgs e) {
        Helper.OpenBrowser("https://github.com/namigop/BeeRock");
    }
}