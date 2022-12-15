using BeeRock.Core.Entities;

namespace BeeRock.Ports.SaveRouteRuleUseCase;

public interface ISaveRouteRuleUseCase {
    Task<string> Save(Rule rule);
}
