using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public interface ILoadServiceRuleSetsUseCase {
    TryAsync<IRestService> LoadById(string docId);
    TryAsync<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource);
}
