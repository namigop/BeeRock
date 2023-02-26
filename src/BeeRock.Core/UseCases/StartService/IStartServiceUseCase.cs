using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.StartService;

public interface IStartServiceUseCase {
    TryAsync<IServerHostingService> Start(ICompiledRestService service);
    TryAsync<IServerHostingService> Start(IRestService service);
}
