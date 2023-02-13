using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IProxyRouteHandler {
    IProxyRouteSelector Selector { get; set; }

    void Begin(ProxyRoute selectedProxyRoute);
    void End(IRoutingMetric metric);
}
