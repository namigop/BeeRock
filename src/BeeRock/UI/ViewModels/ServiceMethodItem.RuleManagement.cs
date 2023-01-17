using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Repository;
using Rule = BeeRock.Core.Entities.Rule;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem {
    public ICommand CreateNewRuleCommand { get; }

    public ICommand DeleteRuleCommand { get; }
    private void OnCreateNewRule() {
        var r = new Rule() {
            IsSelected = false,
            Body = GetDefaultResponse(this.Method),
            Name = $"Rule {this.Rules.Count + 1}",
            StatusCode = 200
        };

        var nRule = new RuleItem(r);
        nRule.Conditions.Clear();
        nRule.Conditions.Add(new WhenConditionItem( new WhenCondition { BoolExpression = "False", IsActive = true }, nRule.Conditions));
        nRule.Refresh();

        this.Rules.Add(nRule);
        this.Method.Rules.Add(nRule.Rule);
        this.SelectedRule = nRule;      
    }

    private void OnDeleteRule(RuleItem ruleItem) {       
        var ruleRepo = new DocRuleRepo(Db.GetRuleDb());
        var isSelected = this.SelectedRule == ruleItem;
        if (isSelected) {
            var pos = this.Rules.IndexOf(ruleItem);
            this.SelectedRule = pos + 1 < (Rules.Count-1 ) ? this.Rules[pos + 1] : this.Rules.FirstOrDefault();
        }

        if (this.Method.Rules.Remove(ruleItem.Rule)) {
            _ = this.Rules.Remove(ruleItem);
            ruleRepo.Delete(ruleItem.DocId);
        }
    }
}
