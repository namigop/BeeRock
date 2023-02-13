using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IProxyRouteSelector {
    ProxyRoute SelectedRouteConfig { get; }
    Uri BuildUri(Uri source);
}
