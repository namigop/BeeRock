using BeeRock.Core.Entities;
using BeeRock.Ports;
using BeeRock.Ports.Repository;
using BeeRock.Ports.SaveRouteRuleUseCase;

namespace BeeRock.Adapters.UseCases.SaveRouteRule;

public class SaveRouteRuleUseCase : UseCaseBase, ISaveRouteRuleUseCase {
    private readonly IDocRuleRepo _repo;

    public SaveRouteRuleUseCase(IDocRuleRepo repo) {
        _repo = repo;
    }

    public async Task<string> Save(Rule rule) {
        var dao = new DocRuleDao {
            IsSelected = rule.IsSelected,
            Name = rule.Name,
            DocId = rule.DocId,
            StatusCode = rule.StatusCode,
            Body = rule.Body,
            Conditions = rule.Conditions.Select(c => ToWhenDao(c)).ToArray()
        };

        if (await _repo.Exists(rule.DocId)) {
            await _repo.Update(dao);
        }
        else {
            var docId = await _repo.Create(dao);
            return docId;
        }

        return rule.DocId;
    }

    private static WhenDao ToWhenDao(WhenCondition whenCondition) {
        return new WhenDao {
            BooleanExpression = whenCondition.BoolExpression,
            IsActive = whenCondition.IsActive
        };
    }
}
