using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarBee {
    public const string VarName = "bee";

    public ScriptingVarBee(string swaggerUrl, string serverMethod, ReadOnlyDictionary<string, object> variables) {
        SwaggerUrl = swaggerUrl;
        ServerMethod = serverMethod;
        Proxy = new ScriptingVarProxy(swaggerUrl, serverMethod, variables);
        Run = new ScriptingVarRun(swaggerUrl, serverMethod);
        FileResp = new ScriptingVarFileResponse();
        Rmq = new ScriptingVarRmq();
        Context = new ScriptingVarContext((HttpContext)variables[RequestHandler.ContextKey]);
    }

    public ScriptingVarContext Context { get; }

    public string ServerMethod { get; }

    public ScriptingVarProxy Proxy { get; }

    public ScriptingVarFileResponse FileResp { get; }
    public ScriptingVarRun Run { get; }

    public ScriptingVarRmq Rmq { get; }
    public string SwaggerUrl { get; }
}
