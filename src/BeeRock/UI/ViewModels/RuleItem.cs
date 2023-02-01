using System.Collections.ObjectModel;
using System.Windows.Input;
using BeeRock.Core.Entities;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class RuleItem : ViewModelBase {
    private string _body;
    private int _delaySec;
    private string _docId;
    private bool _isMatchedByHttpRequest;
    private bool _isSelected;
    private string _name;
    private int _statusCode;

    public RuleItem() : this(new Rule()) {
    }

    public RuleItem(Rule rule) {
        Rule = rule;
        From(rule);
        AddConditionCommand = ReactiveCommand.Create(OnAddCondition);
    }

    public Rule Rule { get; }

    public int DelaySec {
        get => _delaySec;
        set => this.RaiseAndSetIfChanged(ref _delaySec, value);
    }

    public int StatusCode {
        get => _statusCode;
        set => this.RaiseAndSetIfChanged(ref _statusCode, value);
    }

    public bool IsSelected {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public string DocId {
        get => _docId;
        set => this.RaiseAndSetIfChanged(ref _docId, value);
    }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public ObservableCollection<WhenConditionItem> Conditions { get; } = new();

    public string Body {
        get => _body;
        set => this.RaiseAndSetIfChanged(ref _body, value);
    }

    public ICommand AddConditionCommand { get; }

    public bool IsMatchedByHttpRequest {
        get => _isMatchedByHttpRequest;
        set {
            this.RaiseAndSetIfChanged(ref _isMatchedByHttpRequest, value);
            this.RaisePropertyChanged(nameof(RuleMatchedColor));
        }
    }

    public string RuleMatchedColor => _isMatchedByHttpRequest ? "LightGreen" : "White";

    public void From(Rule rule) {
        Body = rule.Body;
        Conditions.Clear();
        _ = rule.Conditions?
            .Select(c => new WhenConditionItem(c, Conditions))
            .Iter(c => Conditions.Add(c));
        Name = rule.Name;
        DocId = rule.DocId;
        IsSelected = rule.IsSelected;
        StatusCode = rule.StatusCode;
        DelaySec = rule.DelayMsec / 1000;
    }

    private void OnAddCondition() {
        Conditions.Add(new WhenConditionItem(new WhenCondition {
            IsActive = true,
            BoolExpression = ""
        }, Conditions));
    }

    public void Refresh() {
        Rule.DelayMsec = DelaySec * 1000;
        Rule.Body = Body;
        Rule.Name = Name;
        Rule.DocId = DocId;
        Rule.IsSelected = IsSelected;
        Rule.StatusCode = StatusCode;
        Rule.Conditions = Conditions
            .Select(i => new WhenCondition {
                BoolExpression = i.BoolExpression,
                IsActive = i.IsActive
            })
            .ToArray();
    }
}
