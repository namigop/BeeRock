using System.Collections.ObjectModel;
using BeeRock.Core.Entities;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class RuleItem : ViewModelBase {
    private string _body;
    private string _docId;
    private bool _isSelected;
    private string _name;
    private int _statusCode;

    public RuleItem() : this(new Rule()) {
    }

    public RuleItem(Rule rule) {
        Rule = rule;
        Body = rule.Body;
        Conditions = rule.Conditions
            .Select(c => new WhenConditionItem(c))
            .Then(c => new ObservableCollection<WhenConditionItem>(c));
        Name = rule.Name;
        DocId = rule.DocId;
        IsSelected = rule.IsSelected;
        StatusCode = rule.StatusCode;
    }

    public Rule Rule { get; }

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

    public void Refresh() {
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
