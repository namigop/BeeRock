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

        // var builder = new ContainerBuilder();
        // var types = typeof(App).Assembly.GetTypes()
        //     .Where(t => t.IsInterface && t.FullName.StartsWith("BeeRock.Ports") || t.FullName.StartsWith("BeeRock.Core.Interfaces"));
        //  foreach (var t in types)
        //       builder.RegisterType(t).InstancePerLifetimeScope();
        // Global.Resolver =  builder.Build();;
        //
        // var scope = Global.Resolver.BeginLifetimeScope();
        // var compiler = scope.Resolve<ICsCompiler>();
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
