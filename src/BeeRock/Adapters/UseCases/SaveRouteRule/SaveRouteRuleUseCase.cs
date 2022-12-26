using BeeRock.Core.Entities;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.SaveRouteRuleUseCase;
using BeeRock.Core.Utils;
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
            var res = Requires.NotNull2<string>(rule, nameof(rule))
                .Bind(() => Requires.NotNullOrEmpty2<string>(rule.Name, nameof(rule.Name)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(rule.Body, nameof(rule.Body)))
                .Bind(() => Requires.NotNullOrEmpty2<WhenCondition, string>(rule.Conditions?.ToList(), nameof(rule.Conditions)));

            if (res.IsFaulted)
                return res;

            var dao = new DocRuleDao {
                DelayMsec = rule.DelayMsec,
                IsSelected = rule.IsSelected,
                Name = rule.Name,
                DocId = rule.DocId,
                StatusCode = rule.StatusCode,
                Body = rule.Body,
                LastUpdated = DateTime.Now,
                Conditions = rule.Conditions.Select(ToWhenDao).ToArray()
            };

            rule.LastUpdated = dao.LastUpdated;

            //Will be assigned a DocId if its a new one
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
