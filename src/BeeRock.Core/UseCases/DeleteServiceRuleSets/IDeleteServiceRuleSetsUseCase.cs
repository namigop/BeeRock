using LanguageExt;

namespace BeeRock.Core.UseCases.DeleteServiceRuleSets;

public interface IDeleteServiceRuleSetsUseCase {
    TryAsync<Unit> Delete(string docId);
}
