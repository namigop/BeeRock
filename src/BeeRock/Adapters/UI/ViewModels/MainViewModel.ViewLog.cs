using System.Reactive;
using BeeRock.Adapters.UI.Views;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private LogWindow _logWindow;
    public ReactiveCommand<Unit, Unit> ViewHttpLogCommand => ReactiveCommand.Create(OnView);

    private void OnView() {
        if (_logWindow != null) {
            _logWindow.Close();
            _logWindow = null;
        }

        _logWindow = new LogWindow();
        _logWindow.Show();
    }
}
