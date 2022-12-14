using System.Windows.Input;
using Avalonia.Threading;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
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
            TempPath = AddNewServiceArgs.TempPath
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

    private TryAsync<(bool, TabItemService)> StartServer(TabItemService tabItem) {
        return async () => {
            var startServiceUseCase = new StartServiceUseCase();
            _startLog = startServiceUseCase.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
            var d = startServiceUseCase.Start(tabItem.RestService);
            var result = await d.Match(
                h => new { Host = h, Error = default(Exception) },
                exc => new { Host = default(IServerHostingService), Error = exc });

            if (result.Error != null) {
                var exception = new Exception("Unable to start the service", result.Error);
                return new Result<(bool, TabItemService)>(exception);
            }

            tabItem.CreateServiceCommands(result.Host);
            return (true, tabItem);
        };
    }

    private TryAsync<TabItemService> SetupTabItems(IRestService svc) {
        return async () => {
            await Dispatcher.UIThread.InvokeAsync(() => {
                var svcItem = new TabItemService(svc) { Main = this };
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
                var t = loadUc.LoadBySwaggerAndName(svc.Name, svc.Settings.SourceSwaggerDoc);
                await t.IfSucc(x => savedRestSvc = x);
            }
            else {
                var t = loadUc.LoadById(svc.DocId);
                await t.IfSucc(x => savedRestSvc = x);
            }

            if (savedRestSvc != null)
                //merge the rules loaded from the DB
                foreach (var m in svc.Methods) {
                    m.Rules.Clear();
                    var savedRules = savedRestSvc.Methods
                        .FirstOrDefault(t => t.RouteTemplate == m.RouteTemplate && t.HttpMethod == m.HttpMethod)?.Rules;
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
