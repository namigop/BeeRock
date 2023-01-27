using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarRun {
    public ScriptingVarRun(string swaggerUrl, string serverMethod) {
        SwaggerUrl = swaggerUrl;
        ServerMethod = serverMethod;
    }

    public string ServerMethod { get; set; }

    public Dictionary<string, object> Variables { get; set; }

    public string SwaggerUrl { get; init; }

    public string Py(string pythonFile) {
        Requires.NotNull(pythonFile, nameof(pythonFile));
        Requires.IsTrue(f => File.Exists(f), pythonFile, nameof(pythonFile));
        var resp = PyEngine.ExecuteFile(pythonFile, SwaggerUrl, ServerMethod, Variables);
        return resp;
    }
}
