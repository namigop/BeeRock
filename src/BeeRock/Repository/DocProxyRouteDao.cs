using BeeRock.Core.Interfaces;

namespace BeeRock.Repository;

public record DocProxyRouteDao : IDoc, IDao {
    public ProxyRoutePartDao From { get; set; }
    public ProxyRoutePartDao To { get; set; }
    public bool IsEnabled { get; set; }
    public int Index { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; }
}
