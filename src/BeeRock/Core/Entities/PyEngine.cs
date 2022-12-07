using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace BeeRock.Core.Entities;

public static class PyEngine {
    private static readonly ScriptEngine ScriptEngine = Python.CreateEngine();

    public static dynamic Evaluate(string expression, Dictionary<string, object> variables) {
        var scope = SetupScope(variables);
        expression = !expression.Contains("return ") ? $@"
def run() :
   return {expression}" : $@"

def run() :
       {expression}";

        ScriptEngine.Execute(expression, scope);
        var d = scope.GetVariable("run");
        var ret = d();
        return ret;
    }

    private static ScriptScope SetupScope(Dictionary<string, object> variables) {
        //var eng = Python.CreateEngine();
        var scope = ScriptEngine.CreateScope();
        if (variables == null)
            return scope;

        foreach (var kvp in variables)
            scope.SetVariable(kvp.Key, kvp.Value);

        return scope;
    }
}
