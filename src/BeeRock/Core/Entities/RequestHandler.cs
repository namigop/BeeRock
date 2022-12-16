using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Avalonia.Controls;
using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;


namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
// ReSharper disable once UnusedType.Global
public static class RequestHandler {
    public static string Handle(string methodName, Dictionary<string, object> variables) {
        //Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        Requires.NotNullOrEmpty(methodName, nameof(methodName));
        Requires.NotNullOrEmpty(variables, nameof(variables));

        //wrap the http request headers for easy access to the .py scripts
        var header = (IHeaderDictionary) variables["header"];
        var wrapper = new ScriptingHttpHeader(header);
        variables["header"] = wrapper;

        var m = Global.CurrentServices
            .SelectMany(c => c.Methods)
            .First(t => t.Method.MethodName == methodName);

        try {
            m.HttpCallIsActive = true;

            //Check the WhenConditions to see whether the request fulfills the conditions
            var canContinue = CheckWhenConditions(m, variables);
            if (!canContinue) {
                m.HttpCallIsOk = false;
                throw new RestHttpException {
                    StatusCode = HttpStatusCode.ServiceUnavailable,
                    Error = "BeeRock error. Unable to match \"When\" conditions.  Please re-check."
                };
            }

            //throws a custom exception that will be handled by the middleware
            if ((int)m.SelectedHttpResponseType.StatusCode >= 400) {
                m.HttpCallIsOk = false;
                throw new RestHttpException {
                    StatusCode = m.SelectedHttpResponseType.StatusCode,
                    Error = ScriptedJson.Evaluate(m.SelectedRule.Body, variables)
                };
            }


            //200 OK
            m.HttpCallIsOk = true;
            return ScriptedJson.Evaluate(m.SelectedRule.Body, variables);
        }
        finally {
            m.HttpCallIsActive = false;

            var headerItem = m.ParamInfoItems.First(p => p.Name == "header");
            var sb = new StringBuilder();
            foreach (var h in header.Keys) {
                sb.AppendLine($"{h} : {header[h]}");
            }

            headerItem.DefaultJson = sb.ToString();
        }
    }

    private static bool CheckWhenConditions(ServiceMethodItem serviceMethodItem, Dictionary<string, object> variables) {
        foreach (var condition in serviceMethodItem.SelectedRule.Conditions.Where(w => w.IsActive)) {
            var result = PyEngine.Evaluate(condition.BoolExpression, variables);
            if (!(bool)result) return false;
        }

        return true;
    }
}
