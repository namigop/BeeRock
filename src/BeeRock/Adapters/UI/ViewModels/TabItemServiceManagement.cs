using System.Collections.ObjectModel;
using System.Windows.Input;
using BeeRock.Adapters.UseCases.LoadServiceRuleSets;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class TabItemServiceManagement : ViewModelBase, ITabItem {
    private readonly IDocServiceRuleSetsRepo _repo;
    private readonly IDocRuleRepo _ruleRepo;
    public string Name { get; set; }
    public ICommand CloseCommand { get; }
    public string TabType { get; } = "ServiceMenagementTab";
    public string HeaderText { get; } = "Admin";

    public TabItemServiceManagement(IDocServiceRuleSetsRepo repo, IDocRuleRepo ruleRepo) {
        _repo = repo;
        _ruleRepo = ruleRepo;
        this.CloseCommand = ReactiveCommand.Create(OnClose);
    }

    private void OnClose() {
        this.Main.TabItems.Remove(this);
    }

    public MainWindowViewModel Main { get; init; }

    public async Task Init() {
        var uc = new LoadServicesUseCase(_repo);
        var services = await uc.GetAll();
        services.Select(s => new ServiceManagementItem(s, _repo, _ruleRepo))
            .Void(s => this.Services = new ObservableCollection<ServiceManagementItem>(s));
    }

    public ObservableCollection<ServiceManagementItem> Services { get; private set; }
}
