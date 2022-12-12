using BeeRock.Ports.Repository;

namespace BeeRock.Ports.SaveRouteRuleUseCase;

public interface ISaveRouteRuleUseCase {
    Task<string> Save(DocRuleDao dao);
}
