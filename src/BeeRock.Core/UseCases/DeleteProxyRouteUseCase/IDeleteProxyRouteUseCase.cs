using LanguageExt;

namespace BeeRock.Core.UseCases.DeleteServiceRuleSets;

public interface IDeleteProxyRouteUseCase {
    TryAsync<Unit> Delete(string docId);
}
