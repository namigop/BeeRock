using BeeRock.Core.Entities;
using LiteDB;

namespace BeeRock.Ports.Repository;

public class DocServiceRuleSetsDao : IDoc {
    public string ServiceName { get; set; }
    public int PortNumber { get; set; }
    public string SourceSwagger { get; set; }
    public RouteRuleSetsDao[] Routes { get; set; }

    [BsonId] public string DocId { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
