using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveRouteRule;

public interface ISaveProxyRouteUseCase {
    TryAsync<string> Save(ProxyRoute proxyRoute);
}
