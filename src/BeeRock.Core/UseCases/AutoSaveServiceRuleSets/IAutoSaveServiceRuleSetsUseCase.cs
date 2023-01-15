using BeeRock.Core.Interfaces;

using LanguageExt;

namespace BeeRock.Core.UseCases.AutoSaveServiceRuleSets;

public interface IAutoSaveServiceRuleSetsUseCase {

    TryAsync<Unit> Start(Func<IRestService> getService);

    TryAsync<Unit> Stop();
}