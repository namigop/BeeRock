using BeeRock.Adapters.UseCases.SaveRouteRule;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.SaveServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;

namespace BeeRock.Adapters.UseCases.SaveServiceRuleSets;

public class SaveServiceRuleSetsUseCase : UseCaseBase, ISaveServiceRuleSetsUseCase {
    private readonly IDocServiceRuleSetsRepo _repo;

    //private readonly IDocRuleRepo _ruleRepo;
    private readonly SaveRouteRuleUseCase _saveRule;

    public SaveServiceRuleSetsUseCase(IDocServiceRuleSetsRepo repo, IDocRuleRepo ruleRepo) {
        _repo = repo;
        //_ruleRepo = ruleRepo;
        _saveRule = new SaveRouteRuleUseCase(ruleRepo);
    }

    public TryAsync<string> Save(IRestService service) {
        return async () => {
            var routes = new List<RouteRuleSetsDao>();
            foreach (var m in service.Methods) {
                var r = await ToRouteRuleSetDao(m);
                routes.Add(r);
            }

            var dao = new DocServiceRuleSetsDao {
                Routes = routes.ToArray(),
                ServiceName = service.Name,
                DocId = service.DocId,
                PortNumber = service.Settings.PortNumber,
                SourceSwagger = service.Settings.SourceSwaggerDoc
            };

            return await Task.Run(() => _repo.Create(dao));
        };
    }

    private async Task<RouteRuleSetsDao> ToRouteRuleSetDao(RestMethodInfo restMethodInfo) {
        //Save the rule sets to the repo then get the docIds
        var ids = new List<string>();
        foreach (var r in restMethodInfo.Rules)
            await _saveRule.Save(r)
                .IfSucc(id => {
                    r.DocId = id;
                    ids.Add(id);
                });

        var dao = new RouteRuleSetsDao {
            HttpMethod = restMethodInfo.HttpMethod,
            MethodName = restMethodInfo.MethodName,
            RouteTemplate = restMethodInfo.RouteTemplate,
            RuleSetIds = ids.ToArray()
        };

        return dao;
    }
}
