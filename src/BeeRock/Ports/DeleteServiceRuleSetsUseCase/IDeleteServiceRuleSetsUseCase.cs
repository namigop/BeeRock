namespace BeeRock.Ports.DeleteServiceRuleSetsUseCase;

public interface IDeleteServiceRuleSetsUseCase {
    Task Delete(string docId);
}
