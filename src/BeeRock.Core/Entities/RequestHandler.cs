using System.Net;
using System.Text;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

//using BeeRock.Adapters.UI.ViewModels;

namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
// ReSharper disable once UnusedType.Global
public static class RequestHandler {
    private const string HeaderKey = "header";

    public static IRestRequestTestArgsProvider TestArgsProvider { get; set; }

    public static string Handle(string methodName, Dictionary<string, object> variables) {
        //Console.WriteLine($"Called RequestHandler.Handle for {methodName}");
        Requires.NotNullOrEmpty(methodName, nameof(methodName));
        Requires.NotNullOrEmpty(variables, nameof(variables));

        //wrap the http request headers for easy access to the .py scripts
        var header = WrapHttpHeaders(variables);
        var methodItem = TestArgsProvider.Find(methodName);

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
            if (methodItem.StatusCode >= 400) {
                methodItem.HttpCallIsOk = false;

                //This will be handled by the middleware which will send the appropriate HTTP response
                throw new RestHttpException {
                    StatusCode = (HttpStatusCode)methodItem.StatusCode,
                    Error = ScriptedJson.Evaluate(methodItem.Body, variables)
                };
            }

            //200 OK
            methodItem.HttpCallIsOk = true;
            // return ScriptedJson.Evaluate(methodItem.SelectedRule.Body, variables);
            return ScriptedJson.Evaluate(methodItem.Body, variables);
        }
        finally {
            methodItem.CallCount += 1;
            UpdateSampleValues(variables, methodItem, header);
        }
    }

    private static void UpdateSampleValues(Dictionary<string, object> variables, IRestRequestTestArgs methodItem, IHeaderDictionary header) {
        foreach (var v in variables)
            if (v.Key == HeaderKey) {
                var sb = new StringBuilder();
                foreach (var h in header.Keys) {
                    sb.AppendLine($"{h} : {header[h]}");
                }

                methodItem.UpdateDefaultValues(v.Key, sb.ToString());
            }
            else {
                methodItem.UpdateDefaultValues(v.Key, JsonConvert.SerializeObject(v.Value, Formatting.Indented));
            }
    }

    private static IHeaderDictionary WrapHttpHeaders(Dictionary<string, object> variables) {
        var header = (IHeaderDictionary)variables[HeaderKey];
        var wrapper = new ScriptingHttpHeader(header);
        variables[HeaderKey] = wrapper;
        return header;
    }

    private static bool CheckWhenConditions(IRestRequestTestArgs serviceMethodItem, Dictionary<string, object> variables) {
        foreach (var condition in serviceMethodItem.ActiveWhenConditions) {
            var result = PyEngine.Evaluate(condition, variables);
            if (!(bool)result) return false;
        }

        return true;
    }
}
