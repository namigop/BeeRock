using BeeRock.Core.Interfaces;

namespace BeeRock.Ports.LoadServiceRuleSetsUseCase;

public interface ILoadServiceRuleSetsUseCase {
    Task<IRestService> LoadById(string docId);
    Task<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource);
}
