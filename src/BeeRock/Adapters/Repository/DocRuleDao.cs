using BeeRock.Core.Entities;
using LiteDB;

namespace BeeRock.Ports.Repository;

public class DocRuleDao :IDoc {
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public WhenDao[] Conditions { get; set; }

    [BsonId] public string DocId { get; set; }
    public int DelayMsec { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
