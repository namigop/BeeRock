using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.LoadServiceRuleSets;
using BeeRock.Core.Utils;

using DynamicData;
using DynamicData.Binding;

using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class TabItemService : ViewModelBase, ITabItem {
    private readonly List<ServiceMethodItem> _internalList;
    private readonly IDocRuleRepo _ruleRepo;
    private string _name = "";
    private string _searchText;
    private ServiceMethodItem _selectedMethod;
    private ServiceCommands _serverCommands;
    private RestServiceSettings _settings;
    private IDocServiceRuleSetsRepo _svcRepo;
    private string _swaggerUrl = "";
    private string _url = "";

    public TabItemService() {
        //for the designer intellisense
    }

    public TabItemService(IRestService svc, IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _ruleRepo = ruleRepo;
        _svcRepo = svcRepo;
        RestService = svc;
        Name = svc.Name;
        _internalList = svc.Methods.Select(r => new ServiceMethodItem(r)).ToList();
        Methods = new ObservableCollection<ServiceMethodItem>(_internalList);
        SelectedMethods = new ObservableCollection<ServiceMethodItem>(_internalList.Take(1));
        CloseCommand = ReactiveCommand.Create(OnClose);

        SelectedMethod = SelectedMethods.First();
        this.WhenAnyValue(t => t.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(FilterMethods)
            .Void(d => disposable.Add(d));

        this.WhenAnyValue(t => t.SelectedMethod)
            .Subscribe(t => {
                ShowSelectedMethod(Methods.Where(c => c.CanShow).ToList());
                _ = LoadSelecteMethod();
            }
            )
            .Void(d => disposable.Add(d));

        Methods.ToObservableChangeSet()
            .AutoRefresh(x => x.CanShow)
            .Subscribe(c => { ShowSelectedMethod(Methods.Where(c => c.CanShow).ToList()); })
            .Void(d => disposable.Add(d));
    }

    public ICommand CloseCommand { get; }
    public string HeaderText => $"{Name} : {Settings.PortNumber}";
    public bool IsServiceHost { get; } = true;
    public MainWindowViewModel Main { get; init; }
    public ObservableCollection<ServiceMethodItem> Methods { get; init; }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public ReactiveCommand<Unit, Unit> OpenSwaggerLinkCommand => ReactiveCommand.Create(OpenSwaggerLink);
    public IRestService RestService { get; }

    public string SearchText {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public ServiceMethodItem SelectedMethod {
        get => _selectedMethod;
        set => this.RaiseAndSetIfChanged(ref _selectedMethod, value);
    }

    public ObservableCollection<ServiceMethodItem> SelectedMethods { get; init; }

    public ServiceCommands ServiceCommands {
        get => _serverCommands;
        set => this.RaiseAndSetIfChanged(ref _serverCommands, value);
    }

    public RestServiceSettings Settings {
        get => _settings;
        set {
            this.RaiseAndSetIfChanged(ref _settings, value);
            SwaggerUrl = $"http://localhost:{_settings.PortNumber}/swagger/index.html";
        }
    }

    public string SwaggerUrl {
        get => _swaggerUrl;
        set => this.RaiseAndSetIfChanged(ref _swaggerUrl, value);
    }

    public string TabType { get; } = "ServiceTab";

    public string Url {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public void CreateServiceCommands(IServerHostingService host) {
        ServiceCommands = new ServiceCommands(host);
    }

    public IRestService Refresh() {
        foreach (var methodItem in Methods) {
            methodItem.Refresh();
        }

        return RestService;
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        foreach (var m in Methods)
            m?.Dispose();

        ServiceCommands?.StopCommand?.Execute(null);
    }

    private async void FilterMethods(string text) {
        //this.Methods.Clear();
        if (string.IsNullOrWhiteSpace(text))
            foreach (var m in Methods) {
                m.CanBeSelected = true;
                await Task.Delay(0);
            }
        else
            foreach (var m in Methods) {
                m.CanBeSelected =
                    m.Method.HttpMethod.ToUpperInvariant().Contains(text.ToUpperInvariant()) ||
                    m.Method.RouteTemplate.ToUpperInvariant().Contains(text.ToUpperInvariant());
                await Task.Delay(0);
            }
    }

    private async Task LoadSelecteMethod() {
        if (SelectedMethod != null) {
            var uc = new LoadRuleSetUseCase(_ruleRepo);
            foreach (var ruleItem in SelectedMethod.Rules.Where(t => !string.IsNullOrWhiteSpace(t.DocId))) {
                if (ruleItem.Body != null) {
                    //skip if already loaded
                    continue;
                }

                var temp = await uc.LoadById(ruleItem.DocId).Match(Result.Create, Result.Error<Rule>);
                if (temp.IsFailed)
                    C.Error(temp.Error.ToString());
                else {
                    ruleItem.From(temp.Value);
                }

                if (ruleItem.Body == null) {
                    //something is wrong. We use default values instead
                    ruleItem.Body = ServiceMethodItem.GetDefaultResponse(SelectedMethod.Method);
                }
            }
        }
    }

    private async Task OnClose() {
        var msg = "Are you sure you want to close this tab?";
        var msBoxStandardWindow = MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                ButtonDefinitions = ButtonEnum.YesNoCancel,
                ContentTitle = "Please Confirm",
                ContentMessage = msg,
                Icon = Icon.Question
            });

        var result = await msBoxStandardWindow.Show();
        if (result == ButtonResult.Yes) {
            Main.TabItems.Remove(this);
            Dispose();
            if (!Main.TabItems.Any()) Main.SelectedTabIndex = 0; //back to add service dialog
        }
    }


    private void OpenSwaggerLink() {
        Helper.OpenBrowser(SwaggerUrl);
    }

    private void ShowSelectedMethod(List<ServiceMethodItem> canShowMethods) {
        if (SelectedMethod != null)
            if (!canShowMethods.Contains(SelectedMethod))
                canShowMethods.Add(SelectedMethod);

        foreach (var m in SelectedMethods.ToList())
            if (!canShowMethods.Contains(m))
                SelectedMethods.Remove(m);

        foreach (var c in canShowMethods)
            if (!SelectedMethods.Contains(c))
                SelectedMethods.Add(c);

        //selected method is expanded
        foreach (var m in SelectedMethods) m.IsExpanded = m == SelectedMethod;
    }
}