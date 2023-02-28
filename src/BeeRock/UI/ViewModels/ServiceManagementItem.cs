using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.DeleteServiceRuleSets;
using BeeRock.Core.UseCases.SaveServiceDetails;
using BeeRock.Core.Utils;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ServiceManagementItem : ViewModelBase {
    private readonly Action<ServiceManagementItem> _removeService;
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private string _name;
    private int _portNumber;
    private string _sourceSwaggerDoc;

    private bool _updateInProgress;
    private readonly IRestService _svc;

    public ServiceManagementItem(IRestService svc, IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo, Action<ServiceManagementItem> removeService) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
        _removeService = removeService;
        _name = svc.Name;
        _portNumber = svc.Settings.PortNumber;
        _sourceSwaggerDoc = svc.Settings.SourceSwaggerDoc;
        DeleteCommand = ReactiveCommand.CreateFromTask<object, Unit>(OnDelete);
        DocId = svc.DocId;
        IsDynamic = svc.IsDynamic;
        _svc = svc;
        this.WhenAnyValue(t => t.Name, t => t.PortNumber, t => t.SourceSwaggerDoc)
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(t => {
                Update();
            })
            .Void(d => disposable.Add(d));
    }

    public bool IsDynamic { get; }

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


    private void Update() {
        if (_updateInProgress)
            return;

        _updateInProgress = true;
        var uc = new SaveServiceDetailsUseCase(_svcRepo);
        _ = uc.Save(DocId, this.Name, this.PortNumber, this.SourceSwaggerDoc, this.IsDynamic)
            .Match(
                ok => { _updateInProgress = false; },
                exc => {
                    _updateInProgress = false;
                    C.Error(exc.ToString());
                });
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
