using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace BeeRock.Core.Entities;

public class PyEngine {
    private static readonly ScriptEngine _scripting = Python.CreateEngine();

    public static dynamic Evaluate(string expression, Dictionary<string, object> variables) {
        var scope = SetupScope(variables);
        expression = $@"def run() :
   return {expression}";

        _scripting.Execute(expression, scope);
        var d = scope.GetVariable("run");
        var ret = d();
        return ret;
    }

    private static ScriptScope SetupScope(Dictionary<string, object> variables) {
        //var eng = Python.CreateEngine();
        var scope = _scripting.CreateScope();
        foreach (var kvp in variables)
            scope.SetVariable(kvp.Key, kvp.Value);

        return scope;
    }
}
