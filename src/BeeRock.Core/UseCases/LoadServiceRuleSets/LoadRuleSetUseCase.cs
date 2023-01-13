using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public class LoadRuleSetUseCase : ILoadRuleSetUseCase {
    private readonly IDocRuleRepo _ruleRepo;

    public LoadRuleSetUseCase(IDocRuleRepo ruleRepo) {
        _ruleRepo = ruleRepo;
    }

    public TryAsync<Rule> LoadById(string docId) {
        return async () => {
            var r = Requires.NotNullOrEmpty2<Rule>(docId, nameof(docId));
            if (r.IsFaulted) {
                return r;
            }

            var ruleDao = await Task.Run(() => _ruleRepo.Read(docId));
            if (ruleDao == null) return null;

            var rule = new Rule {
                Name = ruleDao.Name ?? "Default",
                Body = ruleDao.Body,
                DocId = ruleDao.DocId,
                LastUpdated = ruleDao.LastUpdated,
                DelayMsec = ruleDao.DelayMsec,
                IsSelected = ruleDao.IsSelected,
                StatusCode = ruleDao.StatusCode,
                Conditions = ruleDao.Conditions?
                    .Select(c => new WhenCondition { BoolExpression = c.BooleanExpression, IsActive = c.IsActive })
                    .ToArray()
            };

            return rule;
        };
    }
}
