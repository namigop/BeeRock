using BeeRock.Adapters.UI.Models;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private bool _hasService;
    private bool _isBusy;
    private string _swaggerUrl = "https://petstore.swagger.io/v2/swagger.json";

    public MainWindowViewModel() {
        Services = new ServiceItemCollection();
        HasService = false;
    }

    public ServiceItemCollection Services { get; }

    public string SwaggerUrl {
        get => _swaggerUrl;
        set => this.RaiseAndSetIfChanged(ref _swaggerUrl, value);
    }

    public bool HasService {
        get => _hasService;
        set => this.RaiseAndSetIfChanged(ref _hasService, value);
    }

    public bool IsBusy {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }
}
