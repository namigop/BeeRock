using System.Windows.Input;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using IronPython.Modules;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ServiceCommands  : ViewModelBase {
    private readonly IServerHostingService _host;

    public ServiceCommands(IServerHostingService host) {
        _host = host;
        this.StartCommand = ReactiveCommand.CreateFromTask(OnStart);
        this.StopCommand =ReactiveCommand.CreateFromTask(OnStop);
    }

    private async Task OnStop() {
        await _host.StopServer();
        this.RaisePropertyChanged(nameof(CanStart));
        this.RaisePropertyChanged(nameof(CanStop));

    }

    private async Task OnStart() {
        await _host.StartServer();
        this.RaisePropertyChanged(nameof(CanStart));
        this.RaisePropertyChanged(nameof(CanStop));

    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public bool CanStart {
        get => _host.CanStart;
    }

    public bool CanStop {
        get => _host.CanStop;
    }
}
