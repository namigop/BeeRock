using System.Windows.Input;
using BeeRock.Adapters.Repository;
using BeeRock.Adapters.UseCases.AutoSaveServiceRuleSets;
using BeeRock.Ports.Repository;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private readonly AutoSaveServiceRuleSetsUseCase autoSave;
    private bool _hasService;
    private ITabItem _selectedTabItem;
    private int _selectedTabIndex;

    public MainWindowViewModel() {
        TabItems = new TabItemCollection();
        HasService = false;
        ShowNewServiceCommand = ReactiveCommand.Create(OnShowNewServiceDialog);
        OpenServiceMgmtCommand = ReactiveCommand.CreateFromTask(OnOpenServiceMgmt);
        Global.CurrentServices = TabItems;
        _svcRepo = new DocServiceRuleSetsRepo(Global.DbFile);
        _ruleRepo = new DocRuleRepo(Global.DbFile);
        autoSave = new AutoSaveServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
        AddNewServiceArgs = new AddNewServiceArgs(_svcRepo) {
            AddCommand = AddCommand,
            CancelCommand = CancelCommand
        };
    }

    private async Task OnOpenServiceMgmt() {
        var mgmt = this.TabItems.FirstOrDefault(t => t is TabItemServiceManagement);
        if (mgmt == null) {
            var m = new TabItemServiceManagement(_svcRepo, _ruleRepo) { Main = this };

            this.TabItems.Add(m);
            await m.Init();
            this.SelectedTabItem = m;
        }
    }

    public ICommand OpenServiceMgmtCommand { get; }

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
