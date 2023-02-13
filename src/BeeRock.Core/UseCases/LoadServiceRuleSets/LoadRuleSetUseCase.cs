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
            if (r.IsFaulted) return r;

            var ruleDto = await Task.Run(() => _ruleRepo.Read(docId));
            if (ruleDto == null) return null;

            var rule = new Rule {
                Name = ruleDto.Name ?? "Default",
                Body = ruleDto.Body,
                DocId = ruleDto.DocId,
                LastUpdated = ruleDto.LastUpdated,
                DelayMsec = ruleDto.DelayMsec,
                IsSelected = ruleDto.IsSelected,
                StatusCode = ruleDto.StatusCode,
                Conditions = ruleDto.Conditions?
                    .Select(c => new WhenCondition { BoolExpression = c.BooleanExpression, IsActive = c.IsActive })
                    .ToArray()
            };

            return rule;
        };
    }
}
