namespace BeeRock.Core.Ports.DeleteServiceRuleSetsUseCase;

public interface IDeleteServiceRuleSetsUseCase {
    Task Delete(string docId);
}
