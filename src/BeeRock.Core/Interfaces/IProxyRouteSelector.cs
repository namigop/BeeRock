namespace BeeRock.Core.Interfaces;

public interface IProxyRouteHandler {

    void Report(IRoutingMetric metric);
    IProxyRouteSelector2 Selector { get; set; }
}
