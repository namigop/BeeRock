using Avalonia.Threading;
using BeeRock.Adapters.UseCases.AddService;
using BeeRock.Adapters.UseCases.StartService;
using BeeRock.Core.Entities;
using BeeRock.Ports;
using LanguageExt;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel {
    private IDisposable _addSvcLog;
    private IDisposable _startLog;


    public ReactiveCommand<Unit, Unit> AddCommand => ReactiveCommand.CreateFromTask(OnAdd);
    public ReactiveCommand<Unit, Unit> CancelCommand => ReactiveCommand.Create(OnCancel);

    public AddNewServiceArgs AddNewServiceArgs { get; }


    private TryAsync<RestService> AddService() {
        var addServiceUse = new AddServiceUseCase();
        var addServiceParams = new AddServiceParams {
            Port = AddNewServiceArgs.PortNumber,
            ServiceName = AddNewServiceArgs.ServiceName,
            SwaggerUrl = AddNewServiceArgs.SwaggerFileOrUrl
        };

        _addSvcLog = addServiceUse.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
        return addServiceUse.AddService(addServiceParams);
    }

    private TryAsync<(bool, RestService)> StartServer(RestService svc) {
        var startServiceUseCase = new StartServiceUseCase();
        _startLog = startServiceUseCase.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
        return startServiceUseCase.Start(svc).Map(t => (t, svc));
    }

    private async Task OnAdd() {
        AddNewServiceArgs.IsBusy = true;

        await
            AddService()
                .Bind(StartServer)
                .Match(t => {
                        Dispatcher.UIThread.InvokeAsync(() => {
                            var ok = t.Item1;
                            var svc = t.Item2;
                            var svcItem = new ServiceItem(svc) { Main = this };
                            svcItem.Settings = svc.Settings;
                            Services.Add(svcItem);

                            HasService = ok;
                            SelectedTabIndex = ok ? 1 : 0;
                        });
                    },
                    exc => { AddNewServiceArgs.AddServiceLogMessage = $"Failed. {exc.Message}"; });

        AddNewServiceArgs.IsBusy = false;
        _startLog?.Dispose();
        _addSvcLog?.Dispose();
    }
}