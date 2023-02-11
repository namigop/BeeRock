using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.DeleteServiceRuleSets;

public class DeleteProxyRouteUseCase : IDeleteProxyRouteUseCase {
    private readonly IDocProxyRouteRepo _repo;

    public DeleteProxyRouteUseCase(IDocProxyRouteRepo repo) {
        _repo = repo;
    }

    /// <summary>
    ///     Delete a service stored in the DB
    /// </summary>
    public TryAsync<Unit> Delete(string docId) {
        return async () => {
            var res = Requires.NotNullOrEmpty2<Unit>(docId, nameof(docId));
            if (res.IsFaulted)
                return res;

            await Task.Run(() => { _repo.Delete(docId); });

            C.Info($"Deleted proxy route with ID = {docId}");
            return Unit.Default;
        };
    }
}
