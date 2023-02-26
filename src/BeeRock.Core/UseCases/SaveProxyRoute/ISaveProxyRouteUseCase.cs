using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveProxyRoute;

public interface ISaveProxyRouteUseCase {
    TryAsync<string> Save(ProxyRoute proxyRoute);
}
