using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.DeleteServiceRuleSets;

public class DeleteServiceRuleSetsUseCase : IDeleteServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public DeleteServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public TryAsync<Unit> Delete(string svcDocId) {
        return async () => {
            var res = Requires.NotNullOrEmpty2<Unit>(svcDocId, nameof(svcDocId));
            if (res.IsFaulted)
                return res;

            await Task.Run(() => {
                var svc = _svcRepo.Read(svcDocId);
                foreach (var ruleId in svc.Routes.SelectMany(r => r.RuleSetIds)) _ruleRepo.Delete(ruleId);

                _svcRepo.Delete(svcDocId);
            });

            return Unit.Default;
        };
    }
}
