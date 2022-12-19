using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Ports;
using BeeRock.Ports.LoadServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.UseCases.LoadServiceRuleSets;

public class LoadServicesUseCase : UseCaseBase, ILoadServicesUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public LoadServicesUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    public async Task<List<IRestService>> GetAll() {
        var all = await _svcRepo.All();
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
    }
}