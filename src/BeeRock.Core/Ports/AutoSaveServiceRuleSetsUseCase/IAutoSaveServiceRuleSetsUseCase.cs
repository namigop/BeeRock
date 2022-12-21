using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Ports.AutoSaveServiceRuleSetsUseCase;

public interface IAutoSaveServiceRuleSetsUseCase {
    Task Start(Func<IRestService> getService);
    Task Stop();
}
