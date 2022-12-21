using BeeRock.Core.Entities;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.SaveRouteRuleUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.SaveRouteRule;

public class SaveRouteRuleUseCase : UseCaseBase, ISaveRouteRuleUseCase {
    private readonly IDocRuleRepo _repo;

    public SaveRouteRuleUseCase(IDocRuleRepo repo) {
        _repo = repo;
    }

    public TryAsync<string> Save(Rule rule) {
        return async () => {
            var dao = new DocRuleDao {
                DelayMsec = rule.DelayMsec,
                IsSelected = rule.IsSelected,
                Name = rule.Name,
                DocId = rule.DocId,
                StatusCode = rule.StatusCode,
                Body = rule.Body,
                Conditions = rule.Conditions.Select(ToWhenDao).ToArray()
            };

            return await Task.Run(() => _repo.Create(dao));
        };
    }

    private static WhenDao ToWhenDao(WhenCondition whenCondition) {
        return new WhenDao {
            BooleanExpression = whenCondition.BoolExpression,
            IsActive = whenCondition.IsActive
        };
    }
}
