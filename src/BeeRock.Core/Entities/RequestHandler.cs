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
    public const string FileRespKey = "fileResp";
    public const string HeaderKey = "header";
    public static IRestRequestTestArgsProvider TestArgsProvider { get; set; }

    /// <summary>
    ///     Called when the rest endpoint returns text (like json)
    /// </summary>
    public static string Handle(string methodName, Dictionary<string, object> variables) {
        return HandleInternal(methodName, variables, ScriptedJson.Evaluate);
    }

    /// <summary>
    ///     Called then the rest endpoint returns a file type
    /// </summary>
    public static FileContentResult HandleFileResponse(string methodName, Dictionary<string, object> variables) {
        return HandleInternal(methodName, variables, Scripting.Evaluate<FileContentResult>);
    }

    /// <summary>
    ///     Check that the conditions match the incoming request
    /// </summary>
    private static bool CheckWhenConditions(IRestRequestTestArg arg, Dictionary<string, object> variables) {
        foreach (var condition in arg.ActiveWhenConditions) {
            var result = condition.Trim().ToUpper() == "TRUE" || PyEngine.Evaluate(condition, "not needed", "", variables);
            if (!(bool)result)
                //if any condition fails, no need to evaluate the rest
                return false;
        }

        return true;
    }

    /// <summary>
    ///     build the response based on what is configured in the UI.
    /// </summary>
    private static T HandleInternal<T>(string methodName, Dictionary<string, object> variables, Func<string, string, string, Dictionary<string, object>, T> evaluate) {
        Requires.NotNullOrEmpty(methodName, nameof(methodName));
        Requires.NotNullOrEmpty(variables, nameof(variables));

        //wrap the http request headers for easy access to the .py scripts
        var header = WrapHttpHeaders(variables);
        var methodArgs = TestArgsProvider.Find(methodName);

        try {
            foreach (var arg in methodArgs.Args) {
                //Check the WhenConditions to see whether the request fulfills the conditions
                var ruleMatched = CheckWhenConditions(arg, variables);
                if (ruleMatched) {
                    HandleInternalDelay(arg);
                    HandleInternalErrorResponse(arg, variables);
                    methodArgs.HttpCallIsOk = true;
                    methodArgs.Error = "";
                    return evaluate(arg.Body, methodArgs.SwaggerUrl, methodName, variables);
                }
            }

            //if we reach here then none of the configured rules was matched.
            var error = $"Unable to match the request with any of the {methodArgs.Args.Count} conditions";
            methodArgs.HttpCallIsOk = false;
            methodArgs.Error = error;
            throw new RestHttpException {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Error = error
            };
        }
        catch (RestHttpException restExc) {
            methodArgs.HttpCallIsOk = false;
            methodArgs.Error = restExc.Error;
            throw;
        }
        catch (Exception exc) {
            methodArgs.HttpCallIsOk = false;
            var restExc = Helper.FindRestHttpException(exc);
            if (restExc != null) {
                methodArgs.Error = restExc.Error;
                throw restExc;
            }

            methodArgs.Error = exc.ToString();
            throw;
        }
        finally {
            methodArgs.CallCount += 1;
            UpdateSampleValues(variables, methodArgs, header);
        }
    }


    /// <summary>
    ///     Pause the thread if needed
    /// </summary>
    private static void HandleInternalDelay(IRestRequestTestArg arg) {
        //Check the configured delay and put the current request thread to sleep if needed
        if (arg.DelayMsec > 0)
            Thread.Sleep(arg.DelayMsec);
    }

    /// <summary>
    ///     If the user configured an error response, throw an exception that will
    ///     be handled by a middleware that will convert it to the proper HTTP response
    /// </summary>
    private static void HandleInternalErrorResponse(IRestRequestTestArg arg, Dictionary<string, object> variables) {
        //throws a custom exception that will be handled by the middleware
        if (arg.StatusCode >= 400)
            //This will be handled by the middleware which will send the appropriate HTTP response
            throw new RestHttpException {
                StatusCode = (HttpStatusCode)arg.StatusCode,
                Error = ScriptedJson.Evaluate(arg.Body, "not needed", "", variables)
            };
    }

    /// <summary>
    ///     Update the displayed values with the most recent ones from the request
    /// </summary>
    private static void UpdateSampleValues(Dictionary<string, object> variables, IRestRequestTestArgs methodItem, IHeaderDictionary header) {
        foreach (var v in variables)
            if (v.Key == HeaderKey) {
                var sb = new StringBuilder();
                sb.AppendLine($"Sample usage: {HeaderKey}.Get(\"Key\")");
                sb.AppendLine();
                sb.AppendLine("Http request headers:");
                foreach (var h in header.Keys) sb.AppendLine($"   {h} = {header[h]}");

                methodItem.UpdateDefaultValues(v.Key, sb.ToString());
            }
            else if (v.Key == ScriptingVarBee.VarName) {
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
}
