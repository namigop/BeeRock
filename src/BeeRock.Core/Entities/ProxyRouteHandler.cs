using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public class ProxyRouteHandler : IProxyRouteHandler {
    private readonly Action<ProxyRoute> _onBegin;
    private readonly Action<IRoutingMetric> _onEnd;

    public ProxyRouteHandler(Func<ProxyRoute[]> getRoutingFilters, Action<ProxyRoute> onBegin, Action<IRoutingMetric> onEnd) {
        _onBegin = onBegin;
        _onEnd = onEnd;
        Selector = new ProxyRouteSelector(getRoutingFilters);
    }

    public bool IsTracingEnabled { get; set; } = false;
    public void Begin(ProxyRoute selectedProxyRoute) {
        _onBegin(selectedProxyRoute);
    }

    public void End(IRoutingMetric metric) {
        _onEnd(metric);
    }

    public IProxyRouteSelector Selector { get; set; }
}
