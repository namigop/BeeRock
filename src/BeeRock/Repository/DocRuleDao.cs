using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

using LiteDB;

namespace BeeRock.Repository;

public class DocRuleDao : IDoc, IDao {
    public string Body { get; set; }
    public WhenDao[] Conditions { get; set; }
    public int DelayMsec { get; set; }
    [BsonId] public string DocId { get; set; }
    public bool IsSelected { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public string Name { get; set; }
    public int StatusCode { get; set; }
}