using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveServiceRuleSets;

public interface ISaveServiceRuleSetsUseCase {
    TryAsync<string> Save(IRestService service);
}
