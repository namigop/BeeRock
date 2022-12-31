using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public class LoadServiceRuleSetsUseCase : UseCaseBase, ILoadServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;

    private readonly IDocServiceRuleSetsRepo _svcRepo;


    public LoadServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
    }

    /// <summary>
    ///     Load a stored service by the doc id (GUID)
    /// </summary>
    public TryAsync<IRestService> LoadById(string svcDocId) {
        C.Info($"Loading service with ID = {svcDocId}");
        return async () => {
            var r = Requires.NotNullOrEmpty2<IRestService>(svcDocId, nameof(svcDocId));
            if (r.IsFaulted)
                return r;

            var dto = await Task.Run(() => _svcRepo.Read(svcDocId));
            var svc = Convert(dto);

            //interfaces cannot be lowered so we return Result<T>
            return new Result<IRestService>(svc);
        };
    }

    /// <summary>
    ///     Load a stored service using the service name and its swagger doc
    /// </summary>
    public TryAsync<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource) {
        C.Info($"Loading service with name = {serviceName} and source = {swaggerSource}");
        return async () => {
            var r =
                Requires.NotNullOrEmpty2<IRestService>(serviceName, nameof(serviceName))
                    .Bind(() => Requires.NotNullOrEmpty2<IRestService>(swaggerSource, nameof(swaggerSource)));
            if (r.IsFaulted)
                return r;

            var temp = await Task.Run(() => {
                return _svcRepo.Where(c =>
                    c.SourceSwagger == swaggerSource && c.ServiceName == serviceName);
            });

            var services = temp.ToArray();
            if (services.Any()) {
                //take the first one.
                var svc = Convert(services[0]);
                return new Result<IRestService>(svc);
            }

            return new Result<IRestService>(default(IRestService));
            ;
        };
    }

    /// <summary>
    ///     Convert a service DTO to a domain entity
    /// </summary>
    private IRestService Convert(DocServiceRuleSetsDto dto) {
        var settings = new RestServiceSettings {
            Enabled = true,
            PortNumber = dto.PortNumber,
            SourceSwaggerDoc = dto.SourceSwagger
        };

        var service = new RestService(Array.Empty<Type>(), dto.ServiceName, settings);
        service.DocId = dto.DocId;
        service.LastUpdated = dto.LastUpdated;

        foreach (var d in dto.Routes) {
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
                    DocId = ruleDao.DocId,
                    LastUpdated = ruleDao.LastUpdated,
                    DelayMsec = ruleDao.DelayMsec,
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
