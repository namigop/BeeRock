using BeeRock.Core.Entities;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveRouteRule;

public interface ISaveRouteRuleUseCase {
    TryAsync<string> Save(Rule rule);
}