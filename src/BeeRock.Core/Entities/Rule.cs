using IronPython.Modules;

namespace BeeRock.Core.Entities;

public interface IDoc {
    string DocId { get; set; }
    DateTime LastUpdated { get; set; }
}
public class Rule : IDoc {
    public Rule() {
        Name = "Default";
        IsSelected = true;
        StatusCode = 200;
        DelayMsec = 0;
        DocId = null;
        Conditions = new[] { new WhenCondition { BoolExpression = "True", IsActive = true } };
    }

    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public WhenCondition[] Conditions { get; set; }
    public string DocId { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public int DelayMsec { get; set; }
}
