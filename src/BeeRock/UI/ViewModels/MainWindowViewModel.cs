using System.Windows.Input;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.AutoSaveServiceRuleSets;
using BeeRock.Repository;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly AutoSaveServiceRuleSetsUseCase _autoSave;
    private readonly IDocProxyRouteRepo _proxyRouteRepo;
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private bool _hasService;
    private int _selectedTabIndex;
    private ITabItem _selectedTabItem;
    private TabItemReverseProxy _tabItemReverseProxy;

    public MainWindowViewModel() {
        TabItems = new TabItemCollection();
        HasService = false;
        ShowNewServiceCommand = ReactiveCommand.Create(OnShowNewServiceDialog);
        OpenServiceMgmtCommand = ReactiveCommand.CreateFromTask(OnOpenServiceMgmt);
        OpenReverseProxyCommand = ReactiveCommand.CreateFromTask(OnOpenReverseProxy);
        Global.CurrentServices = TabItems;
        _svcRepo = new DocServiceRuleSetsRepo(Db.GetServiceDb());
        _proxyRouteRepo = new DocProxyRouteRepo(Db.GetProxyRouteDb());
        _ruleRepo = new DocRuleRepo(Db.GetRuleDb());
        _autoSave = new AutoSaveServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
        AddNewServiceArgs = new AddNewServiceArgs(_svcRepo) {
            AddCommand = AddCommand,
            CancelCommand = CancelCommand
        };
    }


    public ICommand OpenServiceMgmtCommand { get; }
    public ICommand OpenReverseProxyCommand { get; }

    public ITabItem SelectedTabItem {
        get => _selectedTabItem;
        set => this.RaiseAndSetIfChanged(ref _selectedTabItem, value);
    }


    public TabItemCollection TabItems { get; }

    public ICommand ShowNewServiceCommand { get; }

    public bool HasService {
        get => _hasService;
        set => this.RaiseAndSetIfChanged(ref _hasService, value);
    }

    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    private async Task OnOpenReverseProxy() {
        var mgmt = TabItems.FirstOrDefault(t => t is TabItemReverseProxy);
        if (mgmt == null) {
            var shouldInit = _tabItemReverseProxy == null;
            _tabItemReverseProxy ??= new TabItemReverseProxy(_proxyRouteRepo) { Main = this };

            TabItems.Add(_tabItemReverseProxy);
            SelectedTabItem = _tabItemReverseProxy;

            if (shouldInit) {
                await _tabItemReverseProxy.Init();
            }
        }
        else {
            SelectedTabItem = mgmt;
        }
    }

    private async Task OnOpenServiceMgmt() {
        var mgmt = TabItems.FirstOrDefault(t => t is TabItemServiceManagement);
        if (mgmt == null) {
            var m = new TabItemServiceManagement(_svcRepo, _ruleRepo) { Main = this };

            TabItems.Add(m);
            await m.Init();
            SelectedTabItem = m;
        }
        else {
            SelectedTabItem = mgmt;
        }
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        Db.Dispose();
    }

    public event EventHandler RequestClose;

    private void OnShowNewServiceDialog() {
        SelectedTabIndex = 0;
    }

    private void OnCancel() {
        if (TabItems.Any())
            SelectedTabIndex = 1; //show the services
        else
            RequestClose?.Invoke(this, null);
    }

    public void Init() {
        try {
            Directory.CreateDirectory(Global.AppDataPath);
            Directory.CreateDirectory(Global.TempPath);
            Directory.GetFiles(Global.TempPath).Iter(File.Delete);

            _ = AddNewServiceArgs.Init();
        }
        catch {
            //ignore. We clean up the folder if possbile
        }
    }
}
