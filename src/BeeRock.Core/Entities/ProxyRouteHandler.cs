using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public class ProxyRouteHandler : IProxyRouteHandler {
    private readonly Action<IRoutingMetric> _receiveMetric;

    public ProxyRouteHandler(Func<ProxyRoute[]> getRoutingFilters, Action<IRoutingMetric> receiveMetric) {
        _receiveMetric = receiveMetric;
        this.Selector = new ProxyRouteSelector(getRoutingFilters);
    }
    public void Report(IRoutingMetric metric) {
        _receiveMetric(metric);
    }

    public IProxyRouteSelector2 Selector { get; set; }
}
