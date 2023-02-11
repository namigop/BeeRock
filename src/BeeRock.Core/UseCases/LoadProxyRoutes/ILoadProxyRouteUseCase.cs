using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public interface ILoadProxyRouteUseCase {
    TryAsync<ProxyRoute> LoadById(string docId);

    TryAsync<List<ProxyRoute>> GetAll();
}
