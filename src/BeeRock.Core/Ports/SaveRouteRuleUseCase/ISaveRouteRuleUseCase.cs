using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.Ports.SaveRouteRuleUseCase;

public interface ISaveRouteRuleUseCase {
    TryAsync<string> Save(Rule rule);
}
