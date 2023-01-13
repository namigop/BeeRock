using System.Windows.Input;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem {

    private string _newRuleName;

    public ICommand CreateNewRuleCommand { get; }

    private void OnCreateNewRule() {
        if (!string.IsNullOrWhiteSpace(NewRuleName)) {
            this.Rules.Add(new RuleItem()
            {
                IsSelected = true,
                Body = GetDefaultResponse(this.Method),
                Name = this.NewRuleName
            });

            this.SelectedRule = this.Rules.Last();
            this.Method.Rules.Add(this.SelectedRule.Rule);
            this.NewRuleName = "";
        }
    }

    public string NewRuleName {
        get => _newRuleName;
        set => this.RaiseAndSetIfChanged(ref _newRuleName , value);
    }
}
