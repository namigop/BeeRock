using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.SaveRouteRule;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.SaveServiceRuleSets;

public class SaveServiceRuleSetsUseCase : UseCaseBase, ISaveServiceRuleSetsUseCase {
    private readonly IDocServiceRuleSetsRepo _repo;

    //private readonly IDocRuleRepo _ruleRepo;
    private readonly SaveRouteRuleUseCase _saveRuleUc;

    public SaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo repo, IDocRuleRepo ruleRepo) {
        _repo = repo;
        _saveRuleUc = new SaveRouteRuleUseCase(ruleRepo);
    }

    /// <summary>
    ///     Save the service and its rules to the DB
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public TryAsync<string> Save(IRestService service) {
        return async () => {
            var routes = new List<RouteRuleSetsDto>();
            foreach (var m in service.Methods) {
                var r = await SaveRouteRuleSetDto(m).Match(Result.Create, Result.Error<RouteRuleSetsDto>);
                if (r.IsFailed) return new Result<string>(r.Error);

                routes.Add(r.Value);
            }

            var dto = new DocServiceRuleSetsDto {
                Routes = routes.ToArray(),
                ServiceName = service.Name,
                DocId = service.DocId,
                PortNumber = service.Settings.PortNumber,
                LastUpdated = DateTime.Now,
                SourceSwagger = service.Settings.SourceSwaggerDoc
            };

            service.LastUpdated = dto.LastUpdated;
            return await Task.Run(() => _repo.Create(dto));
        };
    }

    private TryAsync<RouteRuleSetsDto> SaveRouteRuleSetDto(RestMethodInfo restMethodInfo) {
        return async () => {
            //Save the rule sets to the repo then get the docIds
            var ids = new List<string>();
            foreach (var r in restMethodInfo.Rules) {
                var res = await _saveRuleUc.Save(r).Match(Result.Create, Result.Error<string>);
                if (res.IsFailed && !string.IsNullOrWhiteSpace(r.DocId)) {
                    //this is an existing rule, but has not been loaded.
                    ids.Add(r.DocId);
                    continue;
                }

                r.DocId = res.Value;
                ids.Add(res.Value);
            }

            var dto = new RouteRuleSetsDto {
                HttpMethod = restMethodInfo.HttpMethod,
                MethodName = restMethodInfo.MethodName,
                RouteTemplate = restMethodInfo.RouteTemplate,
                RuleSetIds = ids.ToArray()
            };

            //C.Debug($"Saved rules for {restMethodInfo.HttpMethod} {restMethodInfo.RouteTemplate}");
            return dto;
        };
    }
}
