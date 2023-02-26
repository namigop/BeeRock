using System.Windows.Input;
using Avalonia.Threading;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.AddService;
using BeeRock.Core.UseCases.LoadServiceRuleSets;
using BeeRock.Core.UseCases.StartService;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class MainWindowViewModel {
    private IDisposable _addSvcLog;
    private IDisposable _startLog;


    public ICommand AddCommand => ReactiveCommand.CreateFromTask(OnAdd);
    public ICommand CancelCommand => ReactiveCommand.Create(OnCancel);

    public AddNewServiceArgs AddNewServiceArgs { get; }

    private static ICompiledRestService CreateCompiledService(Type[] controllerTypes, string name, RestServiceSettings settings) {
        return new RestService(controllerTypes, name, settings);
    }

    private static IRestService CreateDynamicService(Type[] controllerTypes, string name, RestServiceSettings settings) {
        return new DynamicRestService(name, settings);
    }

    private TryAsync<IRestService> AddService() {
        int CheckPortUsage(int targetPort) {
            foreach (TabItemService s in TabItems.Where(t => t.IsServiceHost))
                if (s.RestService.Settings.PortNumber == targetPort)
                    return CheckPortUsage(targetPort + 10);

            return targetPort;
        }

        var addServiceParams = new AddServiceParams {
            Port = CheckPortUsage(AddNewServiceArgs.PortNumber),
            ServiceName = AddNewServiceArgs.ServiceName,
            SwaggerUrl = AddNewServiceArgs.SwaggerFileOrUrl,
            DocId = AddNewServiceArgs.DocId,
            TempPath = AddNewServiceArgs.TempPath,
            IsDynamic = string.IsNullOrWhiteSpace(AddNewServiceArgs.SwaggerFileOrUrl)
        };

        var existing =
            AddNewServiceArgs.ServiceSelections
                .FirstOrDefault(r =>
                    r.SwaggerUrlOrFile == AddNewServiceArgs.SwaggerFileOrUrl &&
                    r.Name == AddNewServiceArgs.ServiceName);

        addServiceParams.DocId = existing?.DocId ?? "";


        var addServiceUse = !addServiceParams.IsDynamic ? new AddServiceUseCase(CreateCompiledService) : new AddServiceUseCase(CreateDynamicService);

        _addSvcLog = addServiceUse.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
        return addServiceUse.AddService(addServiceParams);
    }

    private TryAsync<(bool, TabItemService)> StartServer(TabItemService tabItem) {
        return async () => {
            var startServiceUseCase = new StartServiceUseCase();
            _startLog = startServiceUseCase.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);

            var d = startServiceUseCase.Start(tabItem.RestService);
            var result = await d.Match(Result.Create, Result.Error<IServerHostingService>);

            if (result.IsFailed) {
                var exception = new Exception("Unable to start the service", result.Error);
                return new Result<(bool, TabItemService)>(exception);
            }

            tabItem.CreateServiceCommands(result.Value);
            return (true, tabItem);
        };
    }

    private TryAsync<TabItemService> SetupTabItems(IRestService svc) {
        return async () => {
            await Dispatcher.UIThread.InvokeAsync(() => {
                var svcItem = new TabItemService(svc, _svcRepo, _ruleRepo) { Main = this };
                svcItem.Settings = svc.Settings;
                TabItems.Add(svcItem);
                SelectedTabItem = svcItem;
            });

            return new Result<TabItemService>((TabItemService)SelectedTabItem);
        };
    }

    private TryAsync<IRestService> TryLoadFromRepository(IRestService svc) {
        return async () => {
            var loadUc = new LoadServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
            var savedRestSvc = default(IRestService);
            if (string.IsNullOrWhiteSpace(svc.DocId)) {
                var t = loadUc.LoadBySwaggerAndName(svc.Name, svc.Settings.SourceSwaggerDoc, false);
                var resp = await t.Match(Result.Create, Result.Error<IRestService>);
                if (resp.IsFailed) return new Result<IRestService>(resp.Error);

                savedRestSvc = resp.Value;
            }
            else {
                var t = loadUc.LoadById(svc.DocId, false);
                var resp = await t.Match(Result.Create, Result.Error<IRestService>);
                if (resp.IsFailed) return new Result<IRestService>(resp.Error);

                savedRestSvc = resp.Value;
            }

            if (savedRestSvc != null)
                //merge the rules loaded from the DB
                foreach (var m in svc.Methods) {
                    m.Rules.Clear();
                    var savedRules = savedRestSvc.Methods
                        .FirstOrDefault(t => t.RouteTemplate == m.RouteTemplate && t.HttpMethod == m.HttpMethod)
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
                .Bind(SetupTabItems)
                .Bind(StartServer)
                .Match(t => {
                        Dispatcher.UIThread.InvokeAsync(() => {
                            var ok = t.Item1;
                            HasService = ok;
                            SelectedTabIndex = ok ? 1 : 0;
                            _ = _autoSave.Start(() => (SelectedTabItem as TabItemService)?.Refresh()).Invoke();
                        });
                    },
                    exc => {
                        AddNewServiceArgs.AddServiceLogMessage = $"Failed. {exc.Message}";
                        C.Error(exc.ToString());
                    });

        AddNewServiceArgs.IsBusy = false;
        _startLog?.Dispose();
        _addSvcLog?.Dispose();
    }
}
