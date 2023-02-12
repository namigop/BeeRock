namespace BeeRock.Core.Interfaces;

public interface IProxyRouteSelector {
    Uri BuildUri(Uri source);
}