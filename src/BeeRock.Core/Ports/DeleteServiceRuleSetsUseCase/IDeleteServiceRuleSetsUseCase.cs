using LanguageExt;

namespace BeeRock.Core.Ports.DeleteServiceRuleSetsUseCase;

public interface IDeleteServiceRuleSetsUseCase {
    TryAsync<Unit> Delete(string docId);
}
