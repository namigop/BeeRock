using Avalonia.Threading;
using BeeRock.Adapters.UseCases.AddService;
using BeeRock.Adapters.UseCases.LoadServiceRuleSets;
using BeeRock.Adapters.UseCases.StartService;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Interfaces;
using BeeRock.Ports.AddServiceUseCase;
using LanguageExt;
using LanguageExt.Common;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel {
    private IDisposable _addSvcLog;
    private IDisposable _startLog;


    public ReactiveCommand<Unit, Unit> AddCommand => ReactiveCommand.CreateFromTask(OnAdd);
    public ReactiveCommand<Unit, Unit> CancelCommand => ReactiveCommand.Create(OnCancel);

    public AddNewServiceArgs AddNewServiceArgs { get; }

    private TryAsync<IRestService> AddService() {
        var addServiceParams = new AddServiceParams {
            Port = AddNewServiceArgs.PortNumber,
            ServiceName = AddNewServiceArgs.ServiceName,
            SwaggerUrl = AddNewServiceArgs.SwaggerFileOrUrl,
            DocId = AddNewServiceArgs.DocId
        };

        var existing =
            AddNewServiceArgs.ServiceSelections
                .FirstOrDefault(r =>
                    r.SwaggerUrlOrFile == AddNewServiceArgs.SwaggerFileOrUrl &&
                    r.Name == AddNewServiceArgs.ServiceName);

        addServiceParams.DocId = existing?.DocId ?? "";

        var addServiceUse = new AddServiceUseCase(
            SwaggerCodeGen.GenerateControllers,
            (dll, code) => new CsCompiler(dll, code),
            (types, name, settings) => new RestService(types, name, settings)
        );

        _addSvcLog = addServiceUse.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
        return addServiceUse.AddService(addServiceParams);
    }

    private TryAsync<(bool, IRestService)> StartServer(IRestService svc) {
        var startServiceUseCase = new StartServiceUseCase();
        _startLog = startServiceUseCase.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
        return startServiceUseCase.Start(svc).Map(t => (t, svc));
    }

    private TryAsync<IRestService> TryLoadFromRepository(IRestService svc) {
        return async () => {
            var loadUc = new LoadServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
            var savedRestSvc = string.IsNullOrWhiteSpace(svc.DocId) ? await loadUc.LoadBySwaggerAndName(svc.Name, svc.Settings.SourceSwaggerDoc) : await loadUc.LoadById(svc.DocId);

            if (savedRestSvc != null)
                //merge the rules loaded from the DB
                foreach (var m in svc.Methods) {
                    m.Rules.Clear();
                    var savedRules = savedRestSvc.Methods
                        .FirstOrDefault(t => t.RouteTemplate == m.RouteTemplate &&
                                             t.HttpMethod == m.HttpMethod
                        )
                        ?.Rules;
                    if (savedRules != null) m.Rules.AddRange(savedRules);
                }


            return new Result<IRestService>(svc);
        };
    }

    private async Task OnAdd() {
        AddNewServiceArgs.IsBusy = true;

        await
            AddService()
                .Bind(TryLoadFromRepository)
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
                            _ = autoSave.Start(() => SelectedService?.Refresh());
                        });
                    },
                    exc => { AddNewServiceArgs.AddServiceLogMessage = $"Failed. {exc.Message}"; });

        AddNewServiceArgs.IsBusy = false;
        _startLog?.Dispose();
        _addSvcLog?.Dispose();
    }
}
