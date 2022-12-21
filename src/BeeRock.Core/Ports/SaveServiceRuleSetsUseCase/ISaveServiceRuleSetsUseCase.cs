using BeeRock.Core.Interfaces;
using LanguageExt;

namespace BeeRock.Core.Ports.SaveServiceRuleSetsUseCase;

public interface ISaveServiceRuleSetsUseCase {
    TryAsync<string> Save(IRestService service);
}
