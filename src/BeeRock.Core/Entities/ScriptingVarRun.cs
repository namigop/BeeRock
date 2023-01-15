using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

public class ScriptingVarRun {
    public Dictionary<string, object> Variables { get; set; }

    public string Py(string pythonFile) {
        Requires.NotNull(pythonFile, nameof(pythonFile));
        Requires.IsTrue(f => File.Exists(f), pythonFile, nameof(pythonFile));
        var resp = PyEngine.ExecuteFile(pythonFile, Variables);
        return resp;
    }
}