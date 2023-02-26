using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadProxyRoutes;

public class LoadProxyRouteUseCase : ILoadProxyRouteUseCase {
    private readonly IDocProxyRouteRepo _proxyRouteRepo;

    public LoadProxyRouteUseCase(IDocProxyRouteRepo proxyRouteRepo) {
        _proxyRouteRepo = proxyRouteRepo;
    }

    public TryAsync<ProxyRoute> LoadById(string docId) {
        return async () => {
            var r = Requires.NotNullOrEmpty2<ProxyRoute>(docId, nameof(docId));
            if (r.IsFaulted) return r;

            var proxyRouteDto = await Task.Run(() => _proxyRouteRepo.Read(docId));
            if (proxyRouteDto == null) return null;

            var proxyRoute = Convert(proxyRouteDto);
            return proxyRoute;
        };
    }

    public TryAsync<List<ProxyRoute>> GetAll() {
        C.Info("Reading stored proxy routing rules");
        return async () => {
            var all = await Task.Run(() => _proxyRouteRepo.All());
            var services = all.Select(Convert).ToList();
            return services;
        };
    }

    private static ProxyRoute Convert(DocProxyRouteDto proxyRouteDto) {
        var proxyRoute = new ProxyRoute {
            Index = proxyRouteDto.Index,
            DocId = proxyRouteDto.DocId,
            LastUpdated = proxyRouteDto.LastUpdated,
            IsEnabled = proxyRouteDto.IsEnabled,
            From = new ProxyRoutePart {
                Host = proxyRouteDto.From.Host,
                PathTemplate = proxyRouteDto.From.PathTemplate,
                Scheme = proxyRouteDto.From.Scheme
            },
            To = new ProxyRoutePart {
                Host = proxyRouteDto.To.Host,
                PathTemplate = proxyRouteDto.To.PathTemplate,
                Scheme = proxyRouteDto.To.Scheme
            }
        };

        return proxyRoute;
    }
}
