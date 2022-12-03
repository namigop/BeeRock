using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using Bym.Core.Package;
using BeeRock.APP.Services;
using BeeRock.Core.Utils;
using BeeRock.Models;
using BeeRock.Views;
using Microsoft.CodeAnalysis;
using ReactiveUI;

namespace BeeRock.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, Unit> ViewHttpLogCommand => ReactiveCommand.Create(OnView);

    private void OnView() {
        var w = new Window();
        w.Content = new LogControl();
        w.Show();
    }
}
