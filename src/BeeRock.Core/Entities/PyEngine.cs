using System.Collections.ObjectModel;
using BeeRock.Core.Entities.Scripting;
using BeeRock.Core.Utils;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace BeeRock.Core.Entities;

public static class PyEngine {
    private const string imports = @"
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

";

    //script should have a method run with no parameters. This should not be changed
    //otherwise it will break all scripts out there
    private const string scriptMethod = "run";

    private static readonly ScriptEngine ScriptEngine = Python.CreateEngine();

    /// <summary>
    ///     Evaluate a python one-liner expression.  Automatically insert a "return" if missing
    /// </summary>
    public static dynamic Evaluate(string expression, string swaggerUrl, string serverMethod, Dictionary<string, object> variables) {
        Requires.NotNullOrEmpty(expression, nameof(expression));

        var scope = SetupScope(swaggerUrl, serverMethod, variables);
        var isMultiline = expression.Contains(Environment.NewLine);

        //if its a multi-line expression we expect it to have the run() method
        if (isMultiline) {
            var hasRunMethod = expression.Contains("def ") && expression.Contains(" run");
            if (!hasRunMethod)
                throw new Exception($"expression does not contain a run method. {Environment.NewLine}{expression}{Environment.NewLine}");
        }
        else {
            //If the one-liner ends with a semicolon (;) the user wants to execute a statement without a return value
            var ret = expression.TrimEnd().EndsWith(";") ? "" : "return ";
                expression = $@"
{imports}
def run() :
   {ret}{expression}";
        }

        try {
            ScriptEngine.Execute(expression, scope);
            var d = scope.GetVariable(scriptMethod);
            var ret = d();
            return ret;
        }
        catch (Exception exc) {
            var msg = $"Script is:{Environment.NewLine}{expression}{Environment.NewLine}";
            throw new Exception(msg, exc);
        }
    }

    public static string ExecuteFile(string pythonFile, string swaggerUrl, string serverMethod, Dictionary<string, object> variables) {
        var scope = SetupScope(swaggerUrl, serverMethod, variables);
        scope = ScriptEngine.ExecuteFile(pythonFile, scope);
        var d = scope.GetVariable(scriptMethod);
        if (d == null)
            throw new Exception($"Script {Path.GetFileName(pythonFile)} did not define a run() method with no parameters");

        var ret = d();
        return ret;
    }

    private static void AddHelperVariables(string swaggerUrl, string serverMethod, Dictionary<string, object> variables) {
        if (variables != null && !variables.ContainsKey(ScriptingVarBee.VarName)) {
            var temp = new Dictionary<string, object>();
            foreach (var (k, v) in variables)
                temp.Add(k, v);

            var scriptingVarBee = new ScriptingVarBee(swaggerUrl, serverMethod, new ReadOnlyDictionary<string, object>(temp));
            variables[ScriptingVarBee.VarName] = scriptingVarBee;
            scriptingVarBee.Run.Variables = variables;
        }
    }

    /// <summary>
    ///     Setup the scope and inject the variables
    /// </summary>
    private static ScriptScope SetupScope(string swaggerUrl, string serverMethod, Dictionary<string, object> variables) {
        AddHelperVariables(swaggerUrl, serverMethod, variables);
        var scope = ScriptEngine.CreateScope();
        if (variables == null)
            return scope;

        foreach (var kvp in variables)
            scope.SetVariable(kvp.Key, kvp.Value);

        return scope;
    }
}
