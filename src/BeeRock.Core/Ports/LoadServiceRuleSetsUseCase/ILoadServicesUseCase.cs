using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServicesUseCase {
    TryAsync<List<IRestService>> GetAll();
}
