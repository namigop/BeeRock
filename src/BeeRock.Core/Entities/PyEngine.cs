using BeeRock.Core.Utils;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace BeeRock.Core.Entities;

public static class PyEngine {
    //script should have a method run with no parameters
    private const string scriptMethod = "run";

    private static readonly ScriptEngine ScriptEngine = Python.CreateEngine();

    /// <summary>
    ///     Evaluate a python one-liner expression.  Automatically insert a "return" if missing
    /// </summary>
    public static dynamic Evaluate(string expression, Dictionary<string, object> variables) {
        Requires.NotNullOrEmpty(expression, nameof(expression));

        var scope = SetupScope(variables);
        expression = !expression.Contains("return ")
            ? $@"
import clr
import time
import statistics
import datetime
import os
import re
import math
import random
import json
import copy
import System

def run() :
   return {expression}"
            : $@"
import clr
import time
import statistics
import datetime
import os
import re
import math
import random
import json
import copy
import System

def run() :
       {expression}";

        ScriptEngine.Execute(expression, scope);
        var d = scope.GetVariable(scriptMethod);
        var ret = d();
        return ret;
    }

    /// <summary>
    ///     Setup the scope and inject the variables
    /// </summary>
    private static ScriptScope SetupScope(Dictionary<string, object> variables) {
        AddHelperVariables(variables);
        var scope = ScriptEngine.CreateScope();
        if (variables == null)
            return scope;

        foreach (var kvp in variables)
            scope.SetVariable(kvp.Key, kvp.Value);

        return scope;
    }

    private static void AddHelperVariables(Dictionary<string, object> variables) {
        if (variables != null) {
            var scriptingVarBee = new ScriptingVarBee();
            variables[ScriptingVarBee.VarName] = scriptingVarBee;
            scriptingVarBee.Run.Variables = variables;
        }
    }

    public static string ExecuteFile(string pythonFile, Dictionary<string, object> variables) {
        var scope = SetupScope(variables);
        scope = ScriptEngine.ExecuteFile(pythonFile, scope);
        var d = scope.GetVariable(scriptMethod);
        if (d == null)
            throw new Exception($"Script {Path.GetFileName(pythonFile)} did not define a run() method with no parameters");

        var ret = d();
        return ret;
    }
}
