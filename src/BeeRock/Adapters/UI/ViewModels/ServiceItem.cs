using System.Reactive;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class TabItemService {
    public ReactiveCommand<Unit, Unit> OpenSwaggerLinkCommand => ReactiveCommand.Create(OpenSwaggerLink);

    private void OpenSwaggerLink() {
        Helper.OpenBrowser(SwaggerUrl);
    }
}