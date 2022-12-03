using System.Diagnostics;
using System.Reactive;
using Bym.Core.Package;
using BeeRock.APP.Services;
using BeeRock.Core.Utils;
using BeeRock.Models;
using Microsoft.CodeAnalysis;
using ReactiveUI;

namespace BeeRock.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, Unit> OpenSwaggerLinkCommand => ReactiveCommand.Create(OpenSwaggerLink);

    private void OpenSwaggerLink() {
        Helper.OpenBrowser(Service.SwaggerUrl);
    }
}
