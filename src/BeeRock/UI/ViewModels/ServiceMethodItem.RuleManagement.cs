using System.Windows.Input;

using BeeRock.Repository;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem {
    public ICommand CreateNewRuleCommand { get; }

    public ICommand DeleteRuleCommand { get; }
    private void OnCreateNewRule() {

        //The newly aded rules are by default not matching anything. User has to change it in the editor by selecting it
        var nRule = new RuleItem() {
            IsSelected = false,
            Body = GetDefaultResponse(this.Method),
            Name = $"Rule {this.Rules.Count + 1}",
            StatusCode = 200
        };
        nRule.Conditions.Clear();
        nRule.Conditions.Add(new WhenConditionItem() { BoolExpression = "False", IsActive = true });
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
            if (pos + 1 < (Rules.Count-1 )) {
                this.SelectedRule = this.Rules[pos + 1];
            }
            else {
                this.SelectedRule = this.Rules.FirstOrDefault();
            }
        }

        if (this.Method.Rules.Remove(ruleItem.Rule)) {
            _ = this.Rules.Remove(ruleItem);
            ruleRepo.Delete(ruleItem.DocId);
        }
    }
}