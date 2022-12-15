using BeeRock.Core.Entities;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class WhenConditionItem : ReactiveObject {
    private string _boolExpression;
    private bool _isActive;

    public WhenConditionItem() {
    }

    public WhenConditionItem(WhenCondition c) {
        BoolExpression = c.BoolExpression;
        IsActive = c.IsActive;
    }

    public string BoolExpression {
        get => _boolExpression;
        set => this.RaiseAndSetIfChanged(ref _boolExpression, value);
    }

    public bool IsActive {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }
}
