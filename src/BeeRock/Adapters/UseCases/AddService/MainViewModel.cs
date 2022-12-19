﻿using System.Linq.Expressions;
using System.Windows.Input;
using Avalonia.Threading;
using BeeRock.Adapters.UseCases.AddService;
using BeeRock.Adapters.UseCases.LoadServiceRuleSets;
using BeeRock.Adapters.UseCases.StartService;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Interfaces;
using BeeRock.Ports.AddServiceUseCase;
using IronPython.Modules;
using LanguageExt;
using LanguageExt.Common;
using Parlot.Fluent;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel {
    private IDisposable _addSvcLog;
    private IDisposable _startLog;


    public ICommand AddCommand => ReactiveCommand.CreateFromTask(OnAdd);
    public ICommand CancelCommand => ReactiveCommand.Create(OnCancel);

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
        return async () => {
            var startServiceUseCase = new StartServiceUseCase();
            _startLog = startServiceUseCase.AddWatch(msg => AddNewServiceArgs.AddServiceLogMessage = msg);
            var d = startServiceUseCase.Start(svc);

            if (await d.IsSucc()) {
                return (true, svc);
            }

            var exception = new Exception("Unable to start the service");
            return new Result<(bool, IRestService)>(exception);
        };
    }

    private TryAsync<IRestService> SetupTabItems(IRestService svc) {
        return async () => {
            await Dispatcher.UIThread.InvokeAsync(() => {
                var svcItem = new TabItemService(svc) { Main = this };
                svcItem.Settings = svc.Settings;
                TabItems.Add(svcItem);
            });

            return new Result<IRestService>(svc);
        };
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
                        .FirstOrDefault(t => t.RouteTemplate == m.RouteTemplate && t.HttpMethod == m.HttpMethod)?.Rules;
                    if (savedRules != null) {
                        m.Rules.AddRange(savedRules);
                    }
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
                            _ = autoSave.Start(() => (SelectedTabItem as TabItemService)?.Refresh());
                        });
                    },
                    exc => { AddNewServiceArgs.AddServiceLogMessage = $"Failed. {exc.Message}"; });

        AddNewServiceArgs.IsBusy = false;
        _startLog?.Dispose();
        _addSvcLog?.Dispose();
    }
}
