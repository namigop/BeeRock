using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class DocServiceRuleSetsDto : IDoc, IDto {
    public string ServiceName { get; set; }
    public int PortNumber { get; set; }
    public string SourceSwagger { get; set; }
    public RouteRuleSetsDto[] Routes { get; set; }

     public string DocId { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
