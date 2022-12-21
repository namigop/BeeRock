using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServiceRuleSetsUseCase {
    Task<IRestService> LoadById(string docId);
    Task<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource);
}
