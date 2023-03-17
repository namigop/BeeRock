using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarBee {
    public const string VarName = "bee";

    public ScriptingVarBee(string swaggerUrl, string serverMethod, string reqBody, ReadOnlyDictionary<string, object> variables) {
        Proxy = new ScriptingVarProxy(swaggerUrl, serverMethod, variables);
        Run = new ScriptingVarRun(swaggerUrl, serverMethod);
        FileResp = new ScriptingVarFileResponse();
        Rmq = new ScriptingVarRmq();
        Log = new ScriptingLog();
        if (variables.ContainsKey(RequestHandler.ContextKey))
            Context = new ScriptingVarContext((HttpContext)variables[RequestHandler.ContextKey], reqBody);
        else
            Context = new ScriptingVarContext(null, null);

    }

    public ScriptingLog  Log { get; }
    public ScriptingVarContext Context { get; }

    public string ServerMethod => Proxy.ServerMethod;

    public ScriptingVarProxy Proxy { get; }

    public ScriptingVarFileResponse FileResp { get; }
    public ScriptingVarRun Run { get; }

    public ScriptingVarRmq Rmq { get; }

    public string SwaggerUrl => Proxy.SwaggerJson;

    public void Update(string swaggerUrl, string serverMethod) {
        Proxy.ServerMethod = serverMethod;
        Proxy.SwaggerJson = swaggerUrl.Replace("index.html", "v1/swagger.json");
        Run.ServerMethod = serverMethod;
        Run.SwaggerUrl = swaggerUrl;
    }
}
