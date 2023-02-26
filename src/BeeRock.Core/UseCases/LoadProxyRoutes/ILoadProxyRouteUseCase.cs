using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadProxyRoutes;

public interface ILoadProxyRouteUseCase {
    TryAsync<ProxyRoute> LoadById(string docId);

    TryAsync<List<ProxyRoute>> GetAll();
}
