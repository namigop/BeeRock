using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveRouteRule;

public class SaveProxyRouteUseCase : UseCaseBase, ISaveProxyRouteUseCase {
    private readonly IDocProxyRouteRepo _repo;

    public SaveProxyRouteUseCase(IDocProxyRouteRepo repo) {
        _repo = repo;
    }

    /// <summary>
    ///     Save a rule. It will be assigned a DocId (GUID) if it is new.
    /// </summary>
    public TryAsync<string> Save(ProxyRoute proxyRoute) {
        return async () => {
            var res = Requires.NotNull2<string>(proxyRoute, nameof(proxyRoute))
                .Bind(() => Requires.NotNull2<string>(proxyRoute.From, nameof(proxyRoute.From)))
                .Bind(() => Requires.IsTrue2<string>(() => proxyRoute.Index >= 0, nameof(proxyRoute.From), $"Route index ({proxyRoute.Index}) is invalid"))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.From.Host, nameof(proxyRoute.From.Host)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.From.Scheme, nameof(proxyRoute.From.Scheme)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.From.PathTemplate, nameof(proxyRoute.From.PathTemplate)))
                .Bind(() => Requires.NotNull2<string>(proxyRoute.To, nameof(proxyRoute.To)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.To.Host, nameof(proxyRoute.To.Host)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.To.Scheme, nameof(proxyRoute.To.Scheme)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(proxyRoute.To.PathTemplate, nameof(proxyRoute.To.PathTemplate)));

            if (res.IsFaulted)
                return res;

            var dao = new DocProxyRouteDto() {
                Index = proxyRoute.Index,
                DocId = proxyRoute.DocId,
                LastUpdated = DateTime.Now,
                From = new ProxyRoutePartDto() {
                    Host = proxyRoute.From.Host,
                    Scheme = proxyRoute.From.Scheme,
                    PathTemplate = proxyRoute.From.PathTemplate
                },
                To = new ProxyRoutePartDto() {
                    Host = proxyRoute.To.Host,
                    Scheme = proxyRoute.To.Scheme,
                    PathTemplate = proxyRoute.To.PathTemplate
                }
            };

            proxyRoute.LastUpdated = dao.LastUpdated;

            //Will be assigned a DocId if its a new one
            var docId = await Task.Run(() => _repo.Create(dao));
            //C.Debug($"Saved rule \"{rule.Name}\", ID = {docId}");
            return docId;
        };
    }
}
