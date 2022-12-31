using System.Windows.Input;
using BeeRock.Core.Interfaces;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ServiceCommands : ViewModelBase {
    private readonly IServerHostingService _host;

    public ServiceCommands(IServerHostingService host) {
        _host = host;
        StartCommand = ReactiveCommand.CreateFromTask(OnStart);
        StopCommand = ReactiveCommand.CreateFromTask(OnStop);
    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    public bool CanStart => _host.CanStart;

    public bool CanStop => _host.CanStop;

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
}
