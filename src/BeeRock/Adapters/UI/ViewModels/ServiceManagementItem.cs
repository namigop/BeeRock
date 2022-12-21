using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using BeeRock.Adapters.UseCases.DeleteServiceRuleSets;
using BeeRock.Adapters.UseCases.SaveServiceDetails;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceManagementItem : ViewModelBase {
    private readonly Action<ServiceManagementItem> _removeService;
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private string _name;
    private int _portNumber;
    private string _sourceSwaggerDoc;

    private bool _updateInProgress;

    public ServiceManagementItem(IRestService svc, IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo, Action<ServiceManagementItem> removeService) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
        _removeService = removeService;
        _name = svc.Name;
        _portNumber = svc.Settings.PortNumber;
        _sourceSwaggerDoc = svc.Settings.SourceSwaggerDoc;
        DeleteCommand = ReactiveCommand.CreateFromTask<object, Unit>(OnDelete);
        DocId = svc.DocId;

        this.WhenAnyValue(t => t.Name, t => t.PortNumber, t => t.SourceSwaggerDoc)
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(t => {
                var name = t.Item1;
                var port = t.Item2;
                var swagger = t.Item3;
                Update(name, port, swagger);
            })
            .Void(d => disposable.Add(d));
    }

    private string DocId { get; }

    public ICommand DeleteCommand { get; init; }

    public string SourceSwaggerDoc {
        get => _sourceSwaggerDoc;
        set => this.RaiseAndSetIfChanged(ref _sourceSwaggerDoc, value);
    }

    public int PortNumber {
        get => _portNumber;
        set => this.RaiseAndSetIfChanged(ref _portNumber, value);
    }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }


    private void Update(string name, int port, string swagger) {
        if (_updateInProgress)
            return;
        try {
            _updateInProgress = true;
            var uc = new SaveServiceDetailsUseCase(_svcRepo);
            _ = uc.Save(DocId, name, port, swagger);
        }
        finally {
            _updateInProgress = false;
        }
    }

    private async Task<Unit> OnDelete(object arg) {
        if (Convert.ToBoolean(arg)) {
            var uc = new DeleteServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
            var t = uc.Delete(DocId);
            await t.Match(
                o => { _removeService(this); },
                exc => {
                    var msg = "Unable to delete the service. Dang it.";
                    var msBoxStandardWindow = MessageBoxManager
                        .GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                            ButtonDefinitions = ButtonEnum.Ok,
                            ContentTitle = "Please Confirm",
                            ContentMessage = msg,
                            Icon = Icon.Question
                        });

                    _ = msBoxStandardWindow.Show();
                }
            );
        }

        return Unit.Default;
    }
}
