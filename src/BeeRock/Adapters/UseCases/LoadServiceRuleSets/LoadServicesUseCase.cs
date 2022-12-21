using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.LoadServiceRuleSets;

public class LoadServicesUseCase : UseCaseBase, ILoadServicesUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public LoadServicesUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    public TryAsync<List<IRestService>> GetAll() {
        return async () => {
            var all = await Task.Run(() => _svcRepo.All());
            var services = all.Select(dao => {
                    var settings = new RestServiceSettings {
                        Enabled = true,
                        PortNumber = dao.PortNumber,
                        SourceSwaggerDoc = dao.SourceSwagger
                    };

                    var service = new RestService(Array.Empty<Type>(), dao.ServiceName, settings);
                    service.DocId = dao.DocId;
                    return (IRestService)service;
                })
                .ToList();

            return services;
        };
    }
}
