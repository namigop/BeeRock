using System.Collections.ObjectModel;
using System.Windows.Input;
using BeeRock.Core.Entities;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class WhenConditionItem : ReactiveObject {
    private string _boolExpression;
    private bool _isActive;

    public WhenConditionItem() {
    }

    public WhenConditionItem(WhenCondition c, ObservableCollection<WhenConditionItem> parent) {
        BoolExpression = c.BoolExpression;
        IsActive = c.IsActive;
        Parent = parent;
        RemoveCommand = ReactiveCommand.Create(() => Parent.Remove(this));
    }

    public ObservableCollection<WhenConditionItem> Parent { get; init; }

    public string BoolExpression {
        get => _boolExpression;
        set => this.RaiseAndSetIfChanged(ref _boolExpression, value);
    }

    public bool IsActive {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }

    public ICommand RemoveCommand { get; }
}
