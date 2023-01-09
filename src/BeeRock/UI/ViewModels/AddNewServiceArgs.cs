using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using IronPython.Modules;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class AddNewServiceArgs : ViewModelBase {
    private readonly ICommand _addCommand;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private string _addServiceLogMessage = "ready...";
    private ICommand _cancelCommand;
    private bool _isBusy;
    private int _portNumber = 8000;
    private ServiceSelection _selectedService;
    private string _serviceName;
    private string _swaggerFileOrUrl = "https://petstore.swagger.io/v2/swagger.json";

    public AddNewServiceArgs(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
        ServiceSelections = new ObservableCollection<ServiceSelection>();

        this.WhenAnyValue(c => c.SelectedService)
            .Where(t => t != null)
            .Subscribe(t => {
                PortNumber = t.PortNumber;
                ServiceName = t.Name;
                SwaggerFileOrUrl = t.SwaggerUrlOrFile;
                DocId = t.DocId;
            });
    }

    public ServiceSelection SelectedService {
        get => _selectedService;
        set => this.RaiseAndSetIfChanged(ref _selectedService, value);
    }

    public ObservableCollection<ServiceSelection> ServiceSelections { get; init; }

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

    public string DocId { get; set; }
    public string TempPath { get; } = Global.TempPath;

    public async Task Init() {
        var stored = await Task.Run(() => _svcRepo.All());
        foreach (var i in stored)
            ServiceSelections.Add(new ServiceSelection {
                Name = i.ServiceName,
                SwaggerUrlOrFile = i.SourceSwagger,
                PortNumber = i.PortNumber,
                DocId = i.DocId
            });

        if (ServiceSelections.IsEmpty())
            ServiceSelections.Add(new ServiceSelection {
                Name = "My Service",
                PortNumber = _portNumber,
                SwaggerUrlOrFile = _swaggerFileOrUrl,
                DocId = ""
            });

        SelectedService = ServiceSelections.Last();
    }
}
