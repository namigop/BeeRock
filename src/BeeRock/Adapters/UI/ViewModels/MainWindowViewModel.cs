using System.Windows.Input;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private bool _hasService;
    private int _selectedTabIndex;

    public MainWindowViewModel() {
        Services = new ServiceItemCollection();
        HasService = false;
        ShowNewServiceCommand = ReactiveCommand.Create(OnShowNewServiceDialog);
        AddNewServiceArgs = new AddNewServiceArgs {
            AddCommand = AddCommand,
            CancelCommand = CancelCommand
        };

        Global.CurrentServices = Services;
    }

    public ServiceItemCollection Services { get; }

    public ICommand ShowNewServiceCommand { get; }

    public bool HasService {
        get => _hasService;
        set => this.RaiseAndSetIfChanged(ref _hasService, value);
    }

    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    private void OnShowNewServiceDialog() {
        SelectedTabIndex = 0;
    }

    private void OnCancel() {
        if (Services.Any())
            SelectedTabIndex = 1; //show the services
    }
}
