using BeeRock.Core.Ports.DeleteServiceRuleSetsUseCase;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.UseCases.DeleteServiceRuleSets;

public class DeleteServiceRuleSetsUseCase : IDeleteServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public DeleteServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task Delete(string svcDocId) {
        Requires.NotNullOrEmpty(svcDocId, nameof(svcDocId));

        await Task.Run(() => {
            var svc = _svcRepo.Read(svcDocId);
            foreach (var ruleId in svc.Routes.SelectMany(r => r.RuleSetIds)) _ruleRepo.Delete(ruleId);

            _svcRepo.Delete(svcDocId);
        });
    }
}
