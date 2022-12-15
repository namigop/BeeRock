using BeeRock.Adapters.UseCases.SaveRouteRule;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Ports;
using BeeRock.Ports.Repository;
using BeeRock.Ports.SaveServiceRulesUseCase;

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

    public async Task<string> Save(IRestService service) {
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

        if (await _repo.Exists(service.DocId)) {
            await _repo.Update(dao);
        }
        else {
            var docId = await _repo.Create(dao);
            return docId;
        }

        return service.DocId;
    }

    private async Task<RouteRuleSetsDao> ToRouteRuleSetDao(RestMethodInfo restMethodInfo) {
        //Save the rule sets to the repo then get the docIds
        var ids = new List<string>();
        foreach (var r in restMethodInfo.Rules) {
            var id = await _saveRule.Save(r);
            r.DocId = id;
            ids.Add(id);
        }

        var dao = new RouteRuleSetsDao {
            HttpMethod = restMethodInfo.HttpMethod,
            MethodName = restMethodInfo.MethodName,
            RouteTemplate = restMethodInfo.RouteTemplate,
            RuleSetIds = ids.ToArray()
        };

        return dao;
    }
}
