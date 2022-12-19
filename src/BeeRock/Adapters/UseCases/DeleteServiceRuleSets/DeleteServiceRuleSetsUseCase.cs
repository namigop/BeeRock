using BeeRock.Ports.DeleteServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.UseCases.DeleteServiceRuleSets;

public class DeleteServiceRuleSetsUseCase : IDeleteServiceRuleSetsUseCase {
    private readonly IDocServiceRuleSetsRepo _svcRepo;
    private readonly IDocRuleRepo _ruleRepo;

    public DeleteServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task Delete(string svcDocId) {
        var svc = await _svcRepo.Read(svcDocId);
        foreach (var ruleId in svc.Routes.SelectMany(r => r.RuleSetIds)) {
            _ruleRepo.Delete(ruleId);
        }

        await _svcRepo.Delete(svcDocId);
    }
}
