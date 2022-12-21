using BeeRock.Core.Entities;

namespace BeeRock.Core.Ports.SaveRouteRuleUseCase;

public interface ISaveRouteRuleUseCase {
    Task<string> Save(Rule rule);
}
