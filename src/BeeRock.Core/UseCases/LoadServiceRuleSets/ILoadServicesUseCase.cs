using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public interface ILoadServicesUseCase {
    TryAsync<List<IRestService>> GetAll();
}