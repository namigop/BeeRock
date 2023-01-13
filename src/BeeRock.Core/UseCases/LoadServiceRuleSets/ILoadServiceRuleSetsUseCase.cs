using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public interface ILoadServiceRuleSetsUseCase {
    TryAsync<IRestService> LoadById(string docId, bool loadRule);
    TryAsync<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource, bool loadRule);
}
