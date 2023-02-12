using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartService;

public interface IStartReverseProxyUseCase {
    TryAsync<IServerHostingService> Start(RestServiceSettings settings,IProxyRouteHandler proxyRouteSelector);
}
