using System.Reactive.Linq;
using Avalonia.Animation.Animators;
using Avalonia.Threading;
using BeeRock.Adapters.UI.Models;
using BeeRock.Adapters.UseCases.AddService;
using BeeRock.Adapters.UseCases.StartService;
using BeeRock.Core.Entities;
using BeeRock.Ports;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.VisualBasic;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private string _addServiceLogMessage = "ready...";
    private string _name;

    private int _portNumber = 8000;
    private ServiceItem _service;
    public ReactiveCommand<Unit, Unit> AddCommand => ReactiveCommand.CreateFromTask(OnAdd);

    public string AddServiceLogMessage {
        get => _addServiceLogMessage;
        set => this.RaiseAndSetIfChanged(ref _addServiceLogMessage, value);
    }

    public ServiceItem Service {
        get => _service;
        set => this.RaiseAndSetIfChanged(ref _service, value);
    }

    public int PortNumber {
        get => _portNumber;
        set => this.RaiseAndSetIfChanged(ref _portNumber, value);
    }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public void file_monad_example() {
        GetLine()
            .Bind(ReadFile)
            .Bind(PrintStrln)
            .Match(success => Console.WriteLine("SUCCESS"),
                failure => Console.WriteLine("FAILURE"));
    }

    static Try<string> GetLine() {
        Console.Write("File:");
        return () => Console.ReadLine();
    }

    static Try<string> ReadFile(string filePath) => () => File.ReadAllText(filePath);

    static Try<bool> PrintStrln(string line) {
        Console.WriteLine(line);
        return () => true;
    }

    private IDisposable addSvcLog;
    private IDisposable startLog;
    private TryAsync<RestService> AddService() {
        var addServiceUse = new AddServiceUseCase();
        var addServiceParams = new AddServiceParams() {
            Port = _portNumber,
            ServiceName = _name,
            SwaggerUrl = _swaggerUrl
        };

        this.addSvcLog = addServiceUse.AddWatch( msg =>  this.AddServiceLogMessage = msg);
        return addServiceUse.AddService(addServiceParams);
    }

    private TryAsync<bool> StartServer(RestService svc) {
        this.Service = new ServiceItem(svc);
        this.Service.Settings = svc.Settings;
        var startServiceUseCase = new StartServiceUseCase();
        this.startLog = startServiceUseCase.AddWatch( msg =>  this.AddServiceLogMessage = msg);
        return startServiceUseCase.Start(svc);
    }

    private async Task OnAdd() {
        await AddService()
            .Bind(StartServer)
            .Match(ok => {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        HasService = true;
                        Global.Trace.Enabled = true;
                    });
                },
                exc => {
                    this.AddServiceLogMessage = $"Failed. {exc.Message}";
                });

        this.startLog?.Dispose();
        this.addSvcLog?.Dispose();
    }
}

internal class Context {
}
