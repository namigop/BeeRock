using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
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
    private readonly ObservableCollection<ServiceSelection> _serviceSelections;
    private string _docId;
    private readonly string _tempPath = Global.TempPath;
    private bool _createFromSwaggerDoc;

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
                CreateFromSwaggerDoc = !t.IsDynamic;
            });
    }

    public ServiceSelection SelectedService {
        get => _selectedService;
        set => this.RaiseAndSetIfChanged(ref _selectedService, value);
    }

    public ObservableCollection<ServiceSelection> ServiceSelections {
        get => _serviceSelections;
        init => _serviceSelections = value;
    }

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

    public string DocId {
        get => _docId;
        set => _docId = value;
    }

    public string TempPath => _tempPath;

    public bool CreateFromSwaggerDoc {
        get => _createFromSwaggerDoc;
        set => this.RaiseAndSetIfChanged(ref _createFromSwaggerDoc , value);
    }

    public async Task Init() {
        var stored = await Task.Run(() => _svcRepo.All());
        var stored2 = stored.OrderBy(d => d.IsDynamic).ThenBy(d => d.ServiceName);
        var latest = stored.MaxBy(d => d.LastUpdated);
        foreach (var i in stored2)
            ServiceSelections.Add(new ServiceSelection {
                Name = i.ServiceName,
                SwaggerUrlOrFile = i.SourceSwagger,
                PortNumber = i.PortNumber,
                DocId = i.DocId,
                IsDynamic = i.IsDynamic
            });

        if (ServiceSelections.IsEmpty())
            ServiceSelections.Add(new ServiceSelection {
                Name = "My Service",
                PortNumber = _portNumber,
                SwaggerUrlOrFile = _swaggerFileOrUrl,
                DocId = "",
                IsDynamic = false
            });

        SelectedService = latest != null ? ServiceSelections.First(l => l.DocId == latest.DocId) : ServiceSelections.First();
    }
}
