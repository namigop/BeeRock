using System.Net;
using System.Text;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

//using BeeRock.Adapters.UI.ViewModels;

namespace BeeRock.Core.Entities;

//This class will be called via Reflection by the generated REST API controller class
// ReSharper disable once UnusedType.Global
public static class RequestHandler {
    public const string HeaderKey = "header";
    public const string FileRespKey = "fileResp";

    public static IRestRequestTestArgsProvider TestArgsProvider { get; set; }

    /// <summary>
    ///     Called then the rest endpoint returns a file type
    /// </summary>
    public static FileContentResult HandleFileResponse(string methodName, Dictionary<string, object> variables) {
        return HandleInternal(methodName, variables, Scripting.Evaluate<FileContentResult>);
    }

    /// <summary>
    ///     Called when the rest endpoint returns text (like json)
    /// </summary>
    public static string Handle(string methodName, Dictionary<string, object> variables) {
        return HandleInternal(methodName, variables, ScriptedJson.Evaluate);
    }

    /// <summary>
    ///     build the response based on what is configured in the UI.
    /// </summary>
    private static T HandleInternal<T>(string methodName, Dictionary<string, object> variables, Func<string, Dictionary<string, object>, T> evaluate) {
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

            HandleInternalDelay(methodItem);
            HandleInternalErrorResponse(methodItem, variables);

            //200 OK
            methodItem.HttpCallIsOk = true;
            return evaluate(methodItem.Body, variables); //ScriptedJson.Evaluate(methodItem.Body, variables);
        }
        finally {
            methodItem.CallCount += 1;
            UpdateSampleValues(variables, methodItem, header);
        }
    }

    /// <summary>
    ///     If the user configured an error response, throw an exception that will
    ///     be handled by a middleware that will convert it to the proper HTTP response
    /// </summary>
    private static void HandleInternalErrorResponse(IRestRequestTestArgs methodItem, Dictionary<string, object> variables) {
        //throws a custom exception that will be handled by the middleware
        if (methodItem.StatusCode >= 400) {
            methodItem.HttpCallIsOk = false;

            //This will be handled by the middleware which will send the appropriate HTTP response
            throw new RestHttpException {
                StatusCode = (HttpStatusCode)methodItem.StatusCode,
                Error = ScriptedJson.Evaluate(methodItem.Body, variables)
            };
        }
    }

    /// <summary>
    ///     Pause the thread if needed
    /// </summary>
    private static void HandleInternalDelay(IRestRequestTestArgs methodItem) {
        //Check the configured delay and put the current request thread to sleep if needed
        if (methodItem.DelayMsec > 0)
            Thread.Sleep(methodItem.DelayMsec);
    }

    /// <summary>
    ///     Update the displayed values with the most recent ones from the request
    /// </summary>
    private static void UpdateSampleValues(Dictionary<string, object> variables, IRestRequestTestArgs methodItem, IHeaderDictionary header) {
        foreach (var v in variables)
            if (v.Key == HeaderKey) {
                var sb = new StringBuilder();
                foreach (var h in header.Keys) sb.AppendLine($"{h} : {header[h]}");

                methodItem.UpdateDefaultValues(v.Key, sb.ToString());
            }
            else if (v.Key == FileRespKey) {
                //do nothing
            }
            else {
                methodItem.UpdateDefaultValues(v.Key, JsonConvert.SerializeObject(v.Value, Formatting.Indented));
            }
    }

    /// <summary>
    ///     Wrap the headers in a function that can be accessed easily in the script
    /// </summary>
    private static IHeaderDictionary WrapHttpHeaders(Dictionary<string, object> variables) {
        var header = (IHeaderDictionary)variables[HeaderKey];
        var wrapper = new ScriptingHttpHeader(header);
        variables[HeaderKey] = wrapper;
        return header;
    }

    /// <summary>
    ///     Check that the conditions match the incoming request
    /// </summary>
    private static bool CheckWhenConditions(IRestRequestTestArgs serviceMethodItem, Dictionary<string, object> variables) {
        foreach (var condition in serviceMethodItem.ActiveWhenConditions) {
            var result = condition.Trim().ToUpper() == "TRUE" || PyEngine.Evaluate(condition, variables);
            if (!(bool)result)
                //if any condition fails, no need to evaluate the rest
                return false;
        }

        return true;
    }
}
