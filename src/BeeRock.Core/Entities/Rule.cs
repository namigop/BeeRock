using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public record Rule : IDoc {
    public string Body { get; set; }
    public WhenCondition[] Conditions { get; set; } = { new() { BoolExpression = "True", IsActive = true } };
    public int DelayMsec { get; set; } = 0;
    public bool IsSelected { get; set; } = true;
    public string Name { get; set; } = "Default";
    public int StatusCode { get; set; } = 200;
    public string DocId { get; set; } = null;
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
