using BeeRock.Core.Interfaces;

namespace BeeRock.Ports.AutoSaveServiceRuleSetsUseCase;

public interface IAutoSaveServiceRuleSetsUseCase {
    Task Start(Func<IRestService> getService);
    Task Stop();
}
