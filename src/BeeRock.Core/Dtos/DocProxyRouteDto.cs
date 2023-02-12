using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public record DocProxyRouteDto : IDoc, IDto {
    public ProxyRoutePartDto From { get; set; }
    public ProxyRoutePartDto To { get; set; }
    public bool IsEnabled { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; }
    public int Index { get; set; }
}