using System.Windows.Input;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class AddNewServiceArgs : ViewModelBase {
    private readonly ICommand _addCommand;
    private string _addServiceLogMessage = "ready...";
    private ICommand _cancelCommand;
    private bool _isBusy;
    private int _portNumber = 8000;
    private string _serviceName;
    private string _swaggerFileOrUrl = "https://petstore.swagger.io/v2/swagger.json";

    public bool IsBusy {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public string AddServiceLogMessage {
        get => _addServiceLogMessage;
        set => this.RaiseAndSetIfChanged(ref _addServiceLogMessage, value);
    }

    public string SwaggerFileOrUrl {
        get => _swaggerFileOrUrl;
        set => this.RaiseAndSetIfChanged(ref _swaggerFileOrUrl, value);
    }

    public string ServiceName {
        get => _serviceName;
        set => this.RaiseAndSetIfChanged(ref _serviceName, value);
    }

    public int PortNumber {
        get => _portNumber;
        set => this.RaiseAndSetIfChanged(ref _portNumber, value);
    }

    public ICommand AddCommand {
        get => _addCommand;
        init => this.RaiseAndSetIfChanged(ref _addCommand, value);
    }

    public ICommand CancelCommand {
        get => _cancelCommand;
        set => this.RaiseAndSetIfChanged(ref _cancelCommand, value);
    }
}
