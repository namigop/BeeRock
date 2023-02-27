using BeeRock.Core.Entities.Scripting;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public class DynamicRestService : IRestService {
    public DynamicRestService(string name, RestServiceSettings settings) {
        Methods = new List<RestMethodInfo> {
            CreateDefaultMethod(HttpMethod.Get),
            CreateDefaultMethod(HttpMethod.Post),
            CreateDefaultMethod(HttpMethod.Put),
            CreateDefaultMethod(HttpMethod.Delete),
            CreateDefaultMethod(HttpMethod.Patch)
        };
        Name = name;
        Settings = settings;
    }

    public bool IsDynamic => true;

    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public List<RestMethodInfo> Methods { get; }

    public string Name { get; init; }

    public RestServiceSettings Settings { get; init; }

    public string SwaggerUrl { get; } = "";

    public static RestMethodInfo CreateDefaultMethod(HttpMethod httpMethod, string path = "/{enter}/{route}/{template}") {
        var p = new List<ParamInfo> {
            ScriptingVarUtils.GetHeadersParamInfo(),
            ScriptingVarUtils.GetQueryStringParamInfo(),
            ScriptingVarUtils.GetRunParamInfo(),
            ScriptingVarUtils.GetRmqParamInfo(),
            ScriptingVarUtils.GetContextParamInfo(),
            ScriptingVarUtils.GetLogParamInfo()
        };

        return
            new RestMethodInfo {
                Parameters = p,
                Rules = new List<Rule>(),
                HttpMethod = httpMethod.Method,
                IsObsolete = false,
                MethodName = Path.GetRandomFileName(),
                ReturnType = typeof(void),
                RouteTemplate = path
            };
    }
}
