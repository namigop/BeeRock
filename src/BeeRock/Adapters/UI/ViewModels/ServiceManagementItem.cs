using System.Windows.Input;
using AvaloniaEdit.Document;
using BeeRock.Adapters.UseCases.DeleteServiceRuleSets;
using BeeRock.Adapters.UseCases.SaveServiceDetails;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;
using LanguageExt;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceManagementItem : ViewModelBase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private readonly IDocRuleRepo _ruleRepo;
    private string _sourceSwaggerDoc;
    private int _portNumber;
    private string _name;
    private bool _isConfirmDeleteOpen;

    public ServiceManagementItem(IRestService svc, IDocServiceRuleSetsRepo _svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = _svcRepo;
        _ruleRepo = ruleRepo;
        Name = svc.Name;
        PortNumber = svc.Settings.PortNumber;
        SourceSwaggerDoc = svc.Settings.SourceSwaggerDoc;
        DeleteCommand = ReactiveCommand.CreateFromTask<object, Unit>(OnDelete);
        DocId = svc.DocId;
        this.IsConfirmDeleteOpen = false;
        this.WhenAnyValue(t => t.Name, t => t.PortNumber, t => t.SourceSwaggerDoc)
            .Subscribe(t => {
                var name = t.Item1;
                var port = t.Item2;
                var swagger = t.Item3;
                Update(name, port, swagger);
            })
            .Void(d => this.disposable.Add(d));
    }

    public bool IsConfirmDeleteOpen {
        get => _isConfirmDeleteOpen;
        set => this.RaiseAndSetIfChanged(ref _isConfirmDeleteOpen, value);
    }

    private void Update(string name, int port, string swagger) {
        var uc = new SaveServiceDetailsUseCase(_svcRepo);
        uc.Save(this.DocId, name, port, swagger);
    }

    private string DocId { get; init; }

    public ICommand DeleteCommand { get; init; }

    private Task<Unit> OnDelete(object arg) {
        if (Convert.ToBoolean(arg)) {
            var uc = new DeleteServiceRuleSetsUseCase(_svcRepo, _ruleRepo);
            uc.Delete(this.DocId);
        }

        return Task.FromResult(Unit.Default);
    }

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
}
