using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServiceRuleSetsUseCase {
    TryAsync<IRestService> LoadById(string docId);
    TryAsync<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource);
}
