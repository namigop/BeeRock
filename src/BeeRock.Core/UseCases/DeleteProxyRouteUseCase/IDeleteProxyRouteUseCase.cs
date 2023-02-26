using LanguageExt;

namespace BeeRock.Core.UseCases.DeleteProxyRouteUseCase;

public interface IDeleteProxyRouteUseCase {
    TryAsync<Unit> Delete(string docId);
}
