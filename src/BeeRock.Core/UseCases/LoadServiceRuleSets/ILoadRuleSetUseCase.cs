using BeeRock.Core.Entities;

using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public interface ILoadRuleSetUseCase {

    TryAsync<Rule> LoadById(string docId);
}