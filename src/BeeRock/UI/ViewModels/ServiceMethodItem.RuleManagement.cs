using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Repository;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem {
    public ICommand CreateNewRuleCommand { get; }

    public ICommand DeleteRuleCommand { get; }

    private void OnCreateNewRule() {
        var r = new Rule {
            IsSelected = false,
            Body = GetDefaultResponse(Method),
            Name = $"Rule {Rules.Count + 1}",
            StatusCode = 200
        };

        var nRule = new RuleItem(r);
        nRule.Conditions.Clear();
        nRule.Conditions.Add(new WhenConditionItem(new WhenCondition { BoolExpression = "False", IsActive = true }, nRule.Conditions));
        nRule.Refresh();

        Rules.Add(nRule);
        Method.Rules.Add(nRule.Rule);
        SelectedRule = nRule;
    }

    private void OnDeleteRule(RuleItem ruleItem) {
        var ruleRepo = new DocRuleRepo(Db.GetRuleDb());
        var isSelected = SelectedRule == ruleItem;
        if (isSelected) {
            var pos = Rules.IndexOf(ruleItem);
            SelectedRule = pos + 1 < Rules.Count - 1 ? Rules[pos + 1] : Rules.FirstOrDefault();
        }

        if (Method.Rules.Remove(ruleItem.Rule)) {
            _ = Rules.Remove(ruleItem);
            ruleRepo.Delete(ruleItem.DocId);
        }
    }
}
