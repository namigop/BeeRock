using System.Reactive;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, Unit> OpenSwaggerLinkCommand => ReactiveCommand.Create(OpenSwaggerLink);

    private void OpenSwaggerLink() {
        Helper.OpenBrowser(Service.SwaggerUrl);
    }
}