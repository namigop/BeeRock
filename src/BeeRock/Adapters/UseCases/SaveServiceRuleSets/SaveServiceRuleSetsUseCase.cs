using BeeRock.Adapters.UseCases.SaveRouteRule;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.SaveServiceRuleSetsUseCase;
using BeeRock.Ports.Repository;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace BeeRock.Adapters.UseCases.SaveServiceRuleSets;

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
            var routes = new List<RouteRuleSetsDao>();
            foreach (var m in service.Methods) {
                var r = await SaveRouteRuleSetDao(m)
                    .Match(dao => new { Dao = dao, Error = default(Exception) },
                           exc => new { Dao = default(RouteRuleSetsDao), Error = exc });
                if (r.Error != null) {
                    return new Result<string>(r.Error);
                }

                routes.Add(r.Dao);
            }

            var dao = new DocServiceRuleSetsDao {
                Routes = routes.ToArray(),
                ServiceName = service.Name,
                DocId = service.DocId,
                PortNumber = service.Settings.PortNumber,
                LastUpdated = DateTime.Now,
                SourceSwagger = service.Settings.SourceSwaggerDoc
            };

            service.LastUpdated =dao.LastUpdated;
            return await Task.Run(() => _repo.Create(dao));
        };
    }

    private TryAsync<RouteRuleSetsDao> SaveRouteRuleSetDao(RestMethodInfo restMethodInfo) {
        return async () => {
            //Save the rule sets to the repo then get the docIds
            var ids = new List<string>();
            foreach (var r in restMethodInfo.Rules) {
                var res = await _saveRule.Save(r)
                    .Match(id => new{ Id = id, Error=default(Exception)},
                           exc=> new{Id = "", Error =exc} );

                if (res.Error != null)
                    return new Result<RouteRuleSetsDao>(res.Error);

                r.DocId = res.Id;
                ids.Add(res.Id);

            }

            var dao = new RouteRuleSetsDao {
                HttpMethod = restMethodInfo.HttpMethod,
                MethodName = restMethodInfo.MethodName,
                RouteTemplate = restMethodInfo.RouteTemplate,
                RuleSetIds = ids.ToArray()
            };

            return dao;
        };
    }
}
