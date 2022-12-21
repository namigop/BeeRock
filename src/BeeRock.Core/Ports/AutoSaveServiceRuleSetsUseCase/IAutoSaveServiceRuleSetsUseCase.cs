using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.Ports.AutoSaveServiceRuleSetsUseCase;

public interface IAutoSaveServiceRuleSetsUseCase {
    TryAsync<Unit> Start(Func<IRestService> getService);
    TryAsync<Unit> Stop();
}
