using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;

namespace BeeRock.Core.UseCases.SaveRouteRule;

public class SaveRouteRuleUseCase : UseCaseBase, ISaveRouteRuleUseCase {
    private readonly IDocRuleRepo _repo;

    public SaveRouteRuleUseCase(IDocRuleRepo repo) {
        _repo = repo;
    }

    /// <summary>
    ///     Save a rule. It will be assigned a DocId (GUID) if it is new.
    /// </summary>
    public TryAsync<string> Save(Rule rule) {
        return async () => {
            var res = Requires.NotNull2<string>(rule, nameof(rule))
                .Bind(() => Requires.NotNullOrEmpty2<string>(rule.Name, nameof(rule.Name)))
                .Bind(() => Requires.NotNullOrEmpty2<string>(rule.Body, nameof(rule.Body)))
                .Bind(() => Requires.NotNullOrEmpty2<WhenCondition, string>(rule.Conditions?.ToList(), nameof(rule.Conditions)));

            if (res.IsFaulted)
                return res;

            var dao = new DocRuleDto {
                DelayMsec = rule.DelayMsec,
                IsSelected = rule.IsSelected,
                Name = rule.Name,
                DocId = rule.DocId,
                StatusCode = rule.StatusCode,
                Body = rule.Body,
                LastUpdated = DateTime.Now,
                Conditions = rule.Conditions.Select(ToWhenDto).ToArray()
            };

            rule.LastUpdated = dao.LastUpdated;

            //Will be assigned a DocId if its a new one
            var docId = await Task.Run(() => _repo.Create(dao));
            C.Debug($"Saved rule \"{rule.Name}\", ID = {docId}");
            return docId;
        };
    }

    private static WhenDto ToWhenDto(WhenCondition whenCondition) {
        return new WhenDto {
            BooleanExpression = whenCondition.BoolExpression,
            IsActive = whenCondition.IsActive
        };
    }
}