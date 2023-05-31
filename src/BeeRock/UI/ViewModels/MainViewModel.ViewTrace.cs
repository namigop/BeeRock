using System.Reactive;
using BeeRock.UI.Views;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, Unit> ViewReqRespTraceCommand => ReactiveCommand.Create(OnViewReqRespTraceWindow);

    private void OnViewReqRespTraceWindow() {
        var w = new ReqRespTraceWindow();
        w.Show();
    }
}
