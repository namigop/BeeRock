using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class DocServiceRuleSetsDto : IDoc, IDto {
    public bool IsDynamic { get; set; } = false;
    public int PortNumber { get; set; }
    public RouteRuleSetsDto[] Routes { get; set; }
    public string ServiceName { get; set; }
    public string SourceSwagger { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
