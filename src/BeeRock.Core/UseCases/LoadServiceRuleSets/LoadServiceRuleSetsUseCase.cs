using BeeRock.Core.Dtos;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.Scripting;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using LanguageExt;
using LanguageExt.Common;

namespace BeeRock.Core.UseCases.LoadServiceRuleSets;

public class LoadServiceRuleSetsUseCase : UseCaseBase, ILoadServiceRuleSetsUseCase {
    private readonly IDocRuleRepo _ruleRepo;
    private readonly LoadRuleSetUseCase _ruleUc;
    private readonly IDocServiceRuleSetsRepo _svcRepo;

    public LoadServiceRuleSetsUseCase(IDocServiceRuleSetsRepo svcRepo, IDocRuleRepo ruleRepo) {
        _svcRepo = svcRepo;
        _ruleRepo = ruleRepo;
        _ruleUc = new LoadRuleSetUseCase(ruleRepo);
    }

    /// <summary>
    ///     Load a stored service by the doc id (GUID)
    /// </summary>
    public TryAsync<IRestService> LoadById(string svcDocId, bool loadRule) {
        C.Info($"Loading service with ID = {svcDocId}");
        return async () => {
            var r = Requires.NotNullOrEmpty2<IRestService>(svcDocId, nameof(svcDocId));
            if (r.IsFaulted)
                return r;

            var dto = await Task.Run(() => _svcRepo.Read(svcDocId));
            var svc = await Convert(dto, loadRule).Match(Result.Create, Result.Error<IRestService>);

            if (svc.IsFailed) return new Result<IRestService>(svc.Error);

            //interfaces cannot be lowered so we return Result<T>
            return new Result<IRestService>(svc.Value);
        };
    }

    /// <summary>
    ///     Load a stored service using the service name and its swagger doc
    /// </summary>
    public TryAsync<IRestService> LoadBySwaggerAndName(string serviceName, string swaggerSource, bool loadRule) {
        C.Info($"Loading service with name = {serviceName} and source = {swaggerSource}");
        return async () => {
            var r = Requires.NotNullOrEmpty2<IRestService>(serviceName, nameof(serviceName));
            //.Bind(() => Requires.NotNullOrEmpty2<IRestService>(swaggerSource, nameof(swaggerSource)));

            if (r.IsFaulted)
                return r;

            var temp = await Task.Run(() => {
                return _svcRepo.Where(c =>
                    c.SourceSwagger == swaggerSource && c.ServiceName == serviceName);
            });

            var services = temp.ToArray();
            if (services.Any()) {
                //take the first one.
                var svc = await Convert(services[0], loadRule).Match(Result.Create, Result.Error<IRestService>);
                if (!svc.IsFailed) return new Result<IRestService>(svc.Value);
            }

            return new Result<IRestService>(default(IRestService));
            ;
        };
    }

    private static IRestService Init(bool isDynamic, string name, RestServiceSettings settings) {
        if (isDynamic) {
            var d = new DynamicRestService(name, settings);
            d.Methods.Clear();
            return d;
        }

        return new RestService(Array.Empty<Type>(), name, settings);
    }

    /// <summary>
    ///     Convert a service DTO to a domain entity
    /// </summary>
    private TryAsync<IRestService> Convert(DocServiceRuleSetsDto dto, bool loadRule) {
        return async () => {
            var settings = new RestServiceSettings {
                Enabled = true,
                PortNumber = dto.PortNumber,
                SourceSwaggerDoc = dto.SourceSwagger
            };

            var service = Init(dto.IsDynamic, dto.ServiceName, settings);
            service.DocId = dto.DocId;
            service.LastUpdated = dto.LastUpdated;

            foreach (var d in dto.Routes) {
                var m = new RestMethodInfo {
                    RouteTemplate = d.RouteTemplate,
                    HttpMethod = d.HttpMethod,
                    MethodName = d.MethodName,
                    Rules = new List<Rule>(d.RuleSetIds.Length),
                    Parameters = TryCreateParameters(dto.IsDynamic)
                };

                service.Methods.Add(m);

                foreach (var ruleId in d.RuleSetIds) {
                    var rule = default(Rule);
                    if (loadRule) {
                        var r = await _ruleUc.LoadById(ruleId).Match(Result.Create, Result.Error<Rule>);
                        rule = r.Value;
                        if (r.IsFailed)
                            return new Result<IRestService>(r.Error);
                    }
                    else {
                        rule = new Rule { DocId = ruleId, StatusCode = 200 };
                    }

                    m.Rules.Add(rule);
                }
            }

            return new Result<IRestService>(service);
        };
    }

    private static List<ParamInfo> TryCreateParameters(bool serviceIsDynamic) {
        if (serviceIsDynamic) {
            var p = new List<ParamInfo> {
                ScriptingVarUtils.GetHeadersParamInfo(),
                ScriptingVarUtils.GetQueryStringParamInfo(),
                ScriptingVarUtils.GetRunParamInfo(),
                ScriptingVarUtils.GetRmqParamInfo(),
                ScriptingVarUtils.GetContextParamInfo(),
                ScriptingVarUtils.GetLogParamInfo()
            };
            return p;
        }

        return null;
    }
}
