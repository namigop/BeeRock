using System.Reactive;
using Avalonia.Controls;
using BeeRock.Adapters.UI.Views;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, Unit> ViewHttpLogCommand => ReactiveCommand.Create(OnView);

    private void OnView() {
        var w = new Window();
        w.Content = new LogControl();
        w.Show();
    }
}