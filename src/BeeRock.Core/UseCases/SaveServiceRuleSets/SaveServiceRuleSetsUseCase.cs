using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.SaveRouteRule;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.SaveServiceRuleSets;

public class SaveServiceRuleSetsUseCase : UseCaseBase, ISaveServiceRuleSetsUseCase {
    private readonly IDocServiceRuleSetsRepo _repo;

    //private readonly IDocRuleRepo _ruleRepo;
    private readonly SaveRouteRuleUseCase _saveRule;

    public SaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo repo, IDocRuleRepo ruleRepo) {
        _repo = repo;
        _saveRule = new SaveRouteRuleUseCase(ruleRepo);
    }

    public TryAsync<string> Save(IRestService service) {
        return async () => {
            var routes = new List<RouteRuleSetsDto>();
            foreach (var m in service.Methods) {
                var r = await SaveRouteRuleSetDto(m)
                    .Match(dto => new { Dto = dto, Error = default(Exception) },
                        exc => new { Dto = default(RouteRuleSetsDto), Error = exc });
                if (r.Error != null) return new Result<string>(r.Error);

                routes.Add(r.Dto);
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
                var res = await _saveRule.Save(r)
                    .Match(id => new { Id = id, Error = default(Exception) },
                        exc => new { Id = "", Error = exc });

                if (res.Error != null)
                    return new Result<RouteRuleSetsDto>(res.Error);

                r.DocId = res.Id;
                ids.Add(res.Id);
            }

            var dto = new RouteRuleSetsDto {
                HttpMethod = restMethodInfo.HttpMethod,
                MethodName = restMethodInfo.MethodName,
                RouteTemplate = restMethodInfo.RouteTemplate,
                RuleSetIds = ids.ToArray()
            };

            return dto;
        };
    }
}
