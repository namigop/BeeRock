using System.Net;
using System.Text;
using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
// ReSharper disable once UnusedType.Global
public static class RequestHandler {
    private const string HeaderKey = "header";

    public static string Handle(string methodName, Dictionary<string, object> variables) {
        //Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        Requires.NotNullOrEmpty(methodName, nameof(methodName));
        Requires.NotNullOrEmpty(variables, nameof(variables));

        //wrap the http request headers for easy access to the .py scripts
        var header = WrapHttpHeaders(variables);
        var methodItem = FindServiceMethod(methodName);

        try {
            //Check the WhenConditions to see whether the request fulfills the conditions
            var canContinue = CheckWhenConditions(methodItem, variables);
            if (!canContinue) {
                methodItem.HttpCallIsOk = false;
                throw new RestHttpException {
                    StatusCode = HttpStatusCode.ServiceUnavailable,
                    Error = "BeeRock error. Unable to match \"When\" conditions.  Please re-check."
                };
            }

            //throws a custom exception that will be handled by the middleware
            if ((int)methodItem.SelectedHttpResponseType.StatusCode >= 400) {
                methodItem.HttpCallIsOk = false;
                throw new RestHttpException {
                    StatusCode = methodItem.SelectedHttpResponseType.StatusCode,
                    Error = ScriptedJson.Evaluate(methodItem.SelectedRule.Body, variables)
                };
            }

            //200 OK
            methodItem.HttpCallIsOk = true;
            return ScriptedJson.Evaluate(methodItem.SelectedRule.Body, variables);
        }
        finally {
            methodItem.CallCount += 1;
            UpdateSampleValues(variables, methodItem, header);
        }
    }

    private static void UpdateSampleValues(Dictionary<string, object> variables, ServiceMethodItem methodItem, IHeaderDictionary header) {
        foreach (var v in variables) {
            var paramInfoItem = methodItem.ParamInfoItems.First(p => p.Name == v.Key);
            if (v.Key == HeaderKey) {
                var headerItem = methodItem.ParamInfoItems.First(p => p.Name == "header");
                var sb = new StringBuilder();
                foreach (var h in header.Keys) sb.AppendLine($"{h} : {header[h]}");

                headerItem.DefaultJson = sb.ToString();
            }
            else {
                paramInfoItem.DefaultJson = JsonConvert.SerializeObject(v.Value, Formatting.Indented);
            }
        }
    }

    private static IHeaderDictionary WrapHttpHeaders(Dictionary<string, object> variables) {
        var header = (IHeaderDictionary)variables["header"];
        var wrapper = new ScriptingHttpHeader(header);
        variables["header"] = wrapper;
        return header;
    }

    private static ServiceMethodItem FindServiceMethod(string methodName) {
        var m = Global.CurrentServices
            .Where(c => c is TabItemService)
            .Cast<TabItemService>()
            .SelectMany(c => c.Methods)
            .First(t => t.Method.MethodName == methodName);
        return m;
    }

    private static bool CheckWhenConditions(ServiceMethodItem serviceMethodItem, Dictionary<string, object> variables) {
        foreach (var condition in serviceMethodItem.SelectedRule.Conditions.Where(w => w.IsActive)) {
            var result = PyEngine.Evaluate(condition.BoolExpression, variables);
            if (!(bool)result) return false;
        }

        return true;
    }
}
