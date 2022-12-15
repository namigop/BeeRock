namespace BeeRock.Core.Entities;

public class Rule {
    public Rule() {
        Name = "Default";
        IsSelected = true;
        StatusCode = 200;
        Conditions = new[] { new WhenCondition { BoolExpression = "True", IsActive = true } };
    }

    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public WhenCondition[] Conditions { get; set; }
    public string DocId { get; set; }
}
