using System.Collections.ObjectModel;

namespace BeeRock.Core.Entities;

public class ScriptingVarBee {
    public const string VarName = "bee";

    public ScriptingVarBee(string swaggerUrl, string serverMethod, ReadOnlyDictionary<string, object> variables) {
        SwaggerUrl = swaggerUrl;
        ServerMethod = serverMethod;
        Proxy = new ScriptingVarProxy(swaggerUrl, serverMethod, variables);
        Run = new ScriptingVarRun(swaggerUrl, serverMethod);
        FileResp = new ScriptingVarFileResponse();
        Rmq = new ScriptingVarRmq();
    }

    public string ServerMethod { get; }

    public ScriptingVarProxy Proxy { get; }

    public ScriptingVarFileResponse FileResp { get; }
    public ScriptingVarRun Run { get; }

    public ScriptingVarRmq Rmq { get; }
    public string SwaggerUrl { get; }
}
