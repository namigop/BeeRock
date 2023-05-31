using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IProxyRouteSelector {
    (ProxyRoute, Dictionary<string, string>) FindMatchingRoute(Uri source);
    Uri BuildUri(Uri source, ProxyRoute routeConfig, Dictionary<string, string> routeParameters);
}
