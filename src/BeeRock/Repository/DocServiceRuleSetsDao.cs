using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

using LiteDB;

namespace BeeRock.Repository;

public class DocServiceRuleSetsDao : IDoc, IDao {
    [BsonId] public string DocId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public int PortNumber { get; set; }
    public RouteRuleSetsDao[] Routes { get; set; }
    public string ServiceName { get; set; }
    public string SourceSwagger { get; set; }
}