using BeeRock.Models;
using ReactiveUI;

namespace BeeRock.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public ServiceItemCollection Services { get; }

    private string swaggerUrl = "https://petstore.swagger.io/v2/swagger.json";

    public string SwaggerUrl {
        get => swaggerUrl;
        set => this.RaiseAndSetIfChanged(ref swaggerUrl, value);
    }

    private bool hasService = false;

    public bool HasService {
        get => hasService;
        set => this.RaiseAndSetIfChanged(ref hasService, value);
    }

    public MainWindowViewModel() {
        Services = new ServiceItemCollection();
        HasService = false;
        //System.Reflection.MethodInfo method= System.Reflection.Assembly.GetEntryAssembly().GetType("BeeRock.Core.Utils.RequestHandler").GetMethod("Handle");
    }
}
