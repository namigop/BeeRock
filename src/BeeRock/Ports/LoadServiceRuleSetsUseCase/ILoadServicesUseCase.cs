using BeeRock.Core.Interfaces;

namespace BeeRock.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServicesUseCase {
    Task<List<IRestService>> GetAll();
}