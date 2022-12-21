using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServicesUseCase {
    Task<List<IRestService>> GetAll();
}