using BeeRock.Core.Interfaces;

namespace BeeRock.Ports.SaveServiceRulesUseCase;

public interface ISaveServiceRuleSetsUseCase {
    Task<string> Save(IRestService service);
}
