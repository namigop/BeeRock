using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Ports.SaveServiceRuleSetsUseCase;

public interface ISaveServiceRuleSetsUseCase {
    Task<string> Save(IRestService service);
}
