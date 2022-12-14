using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using LiteDB;

namespace BeeRock.Repository;

public class DocServiceRuleSetsDao : IDoc, IDao {
    public string ServiceName { get; set; }
    public int PortNumber { get; set; }
    public string SourceSwagger { get; set; }
    public RouteRuleSetsDao[] Routes { get; set; }

    [BsonId] public string DocId { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
