using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public class LoadServicesUseCase : UseCaseBase, ILoadServicesUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public LoadServicesUseCase(IDocServiceRuleSetsRepo svcRepo) {
        _svcRepo = svcRepo;
    }

    /// <summary>
    ///     Get all stored services but do not load its rules
    /// </summary>
    public TryAsync<List<IRestService>> GetAll() {
        C.Info("Reading stored services");
        return async () => {
            var all = await Task.Run(() => _svcRepo.All());
            var services = all.Select(dao => {
                    var settings = new RestServiceSettings {
                        Enabled = true,
                        PortNumber = dao.PortNumber,
                        SourceSwaggerDoc = dao.SourceSwagger
                    };

                    C.Info($"  Found {dao.ServiceName}:{dao.PortNumber}:{dao.SourceSwagger} with ID {dao.DocId}");
                    var service = Init(dao.IsDynamic, dao.ServiceName, settings);
                    service.DocId = dao.DocId;
                    return (IRestService)service;
                })
                .ToList();

            return services;
        };
    }

    public static IRestService Init(bool isDynamic, string name, RestServiceSettings settings) {
        if (isDynamic) {
            var d = new DynamicRestService(name, settings);
            d.Methods.Clear(); //remove the default routes. It will be replaced by the ones stored in the DB
            return d;
        }

        return new RestService(Array.Empty<Type>(), name, settings);
    }
}
