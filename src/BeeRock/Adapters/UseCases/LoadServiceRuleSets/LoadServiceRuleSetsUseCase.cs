using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Ports;
using BeeRock.Core.Ports.LoadServiceRuleSetsUseCase;
using BeeRock.Core.Utils;
using BeeRock.Ports.Repository;

namespace BeeRock.Adapters.UseCases.LoadServiceRuleSets;

public class LoadServiceRuleSetsUseCase : UseCaseBase, ILoadServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;

    private readonly IDocServiceRuleSetsRepo _svcRepo;


    public LoadServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task<IRestService> LoadById(string docId) {
        Requires.NotNullOrEmpty(docId, nameof(docId));
        var dao = await Task.Run(() => _svcRepo.Read(docId));
        return Convert(dao);
    }

    public async Task<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource) {
        Requires.NotNullOrEmpty(serviceName, nameof(serviceName));
        Requires.NotNullOrEmpty(swaggerSource, nameof(swaggerSource));
        var temp = await Task.Run(() => {
            return _svcRepo.Where(c =>
                c.SourceSwagger == swaggerSource && c.ServiceName == serviceName);
        });

        var services = temp.ToArray();
        if (services.Any())
            //take the first one.
            return Convert(services[0]);

        return null;
    }

    private IRestService Convert(DocServiceRuleSetsDao dao) {
        var settings = new RestServiceSettings {
            Enabled = true,
            PortNumber = dao.PortNumber,
            SourceSwaggerDoc = dao.SourceSwagger
        };

        var service = new RestService(Array.Empty<Type>(), dao.ServiceName, settings);
        service.DocId = dao.DocId;

        foreach (var d in dao.Routes) {
            var m = new RestMethodInfo {
                RouteTemplate = d.RouteTemplate,
                HttpMethod = d.HttpMethod,
                MethodName = d.MethodName,
                Rules = new List<Rule>(d.RuleSetIds.Length)
            };
            service.Methods.Add(m);

            foreach (var ruleId in d.RuleSetIds) {
                var ruleDao = _ruleRepo.Read(ruleId);
                if (ruleDao == null)
                    continue;

                var rule = new Rule {
                    Name = ruleDao.Name ?? "Default",
                    Body = ruleDao.Body,
                    IsSelected = ruleDao.IsSelected,
                    StatusCode = ruleDao.StatusCode,
                    Conditions = ruleDao.Conditions
                        .Select(c => new WhenCondition { BoolExpression = c.BooleanExpression, IsActive = c.IsActive })
                        .ToArray()
                };
                m.Rules.Add(rule);
            }
        }

        return service;
    }
}
