using System.Collections.ObjectModel;
using System.Reflection;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BeeRock.Core.Entities.Scripting;

public class ScriptingVarProxy {
    public const string BeeRockClient = "BeeRockClient";
    private static readonly Dictionary<string, Type> ClientTypeCache = new();
    private readonly ReadOnlyDictionary<string, object> _variables;

    public ScriptingVarProxy(string swaggerUrl, string serverMethod, ReadOnlyDictionary<string, object> variables) {
        _variables = variables;
        //http://localhost:8900/swagger/index.html
        SwaggerJson = swaggerUrl.Replace("index.html", "v1/swagger.json");
        ServerMethod = serverMethod;
    }

    public string ServerMethod { get; set; }
    public string SwaggerJson { get; set; }

    public string ForwardToServer(string baseUrl) {
        return ForwardToInternal(baseUrl, false);
    }

    private string ForwardToInternal(string url, bool isForwardingToFull) {
        Requires.NotNullOrEmpty(url, nameof(url));
        Requires.IsTrue(() => url.StartsWith("http"), nameof(url), $"{url} is not a valid URL");

        var clientType = GetClientType(SwaggerJson);
        var mi = FindMethod(ServerMethod, clientType, _variables);

        dynamic client = Activator.CreateInstance(clientType);
        client.RequestHeaders = ((ScriptingHttpHeader)_variables[RequestHandler.HeaderKey]).Request.Headers;
        client.Response = ((HttpContext)_variables[RequestHandler.ContextKey]).Response;
        client.TargetUrl = url;
        client.IsForwardingToFullUrl = isForwardingToFull;

        var args = BuildArgs(mi, _variables);
        try {
            var respObject = mi.Invoke(client, args).ConfigureAwait(false).GetAwaiter().GetResult();
            return mi.ReturnParameter == null ? "{}" : (string)Helper.Serialize(respObject, mi.ReturnType);
        }
        catch {
            //The generated NSwag client will raise exceptions for non 200 OK responses but
            //we will ignore any exceptions because in the (static) ProcessResponse method, the proxied response
            //is already written to the Context.Items as a PassThrough response to be handled by the middleware
            return "";
        }
    }

    private static Type GetClientType(string swaggerDoc) {
        lock (BeeRockClient) {
            if (!ClientTypeCache.ContainsKey(swaggerDoc)) ClientTypeCache[swaggerDoc] = SwaggerCodeGen.GenerateClient(swaggerDoc);
        }

        return ClientTypeCache[swaggerDoc];
    }

    /// <summary>
    ///     Forwards the call to a targetUrl
    /// </summary>
    public string ForwardToUrl(string targetUrl) {
        return ForwardToInternal(targetUrl, true);
    }

    private static List<object> GetMethodArgsFromVariables(ReadOnlyDictionary<string, object> variables) {
        //The variable dictionary contains the AspnetCore and BeeRock types. We want to ignore those
        //so that we can get only the variables needed to call the client method
        var methodArguments = variables.Values.Where(v => {
            if (v != null) {
                var typeName = v.GetType().FullName;
                var isAspNetCoreType = typeName.StartsWith("Microsoft.AspNetCore");
                if (isAspNetCoreType)
                    return false;

                var isBeeRockType = typeName.StartsWith("BeeRock.");
                if (isBeeRockType)
                    return false;
            }

            return true;
        }).ToList();
        return methodArguments;
    }

    private static object[] BuildArgs(MethodInfo mi, ReadOnlyDictionary<string, object> variables) {
        var parameters = mi.GetParameters();
        var methodArguments = GetMethodArgsFromVariables(variables);
        var methodParameters = parameters.Select((pi, index) => {
            return methodArguments[index]
                .Then(obj => Helper.Serialize(obj, obj?.GetType() ?? pi.ParameterType))
                .Then(json => Helper.Deserialize(json, pi.ParameterType));
        }).ToArray();

        return methodParameters;
    }

    private static MethodInfo FindMethod(string serverMethod, Type clientType, ReadOnlyDictionary<string, object> variables) {
        //server method has the format M{MethodName}_{lineNumber} but the client method
        //will have the format {MethodName}Async
        var clientMethodName = serverMethod
            .Split("_")[0]
            .Then(m => m.Substring(1) + "Async");

        foreach (var m in clientType.GetMethods()) {
            var parameters = m.GetParameters();
            if (m.Name == clientMethodName) {
                if (parameters.IsEmpty())
                    return m;
                if (parameters.LastOrDefault().ParameterType != typeof(CancellationToken)) {
                    var methodArgs = GetMethodArgsFromVariables(variables);
                    if (methodArgs.Count == parameters.Length)
                        return m;
                }
            }
        }

        throw new Exception($"Unable to find matching client method for {serverMethod}");
    }


    /// <summary>
    ///     Checks the proxied call status code and raises the RestHttpException (for 4xx and 5xx)
    /// </summary>
    public static void ProcessResponse(HttpClient client, HttpResponseMessage proxyResponse, HttpResponse response) {
        response.Headers.Clear();
        foreach (var h in proxyResponse.Headers) {
            if (h.Key.StartsWith("Transfer-")) //ASP.NET core will add this later.
                continue;

            response.Headers.TryAdd(h.Key, new StringValues(h.Value?.ToArray()));
        }

        //All proxied responses, whether OK or 4xx/5xx, are set as pass through.
        response.HttpContext.Items[nameof(PassThroughResponse)] = new PassThroughResponse {
            Content = proxyResponse.Content != null
                ? proxyResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()
                : null,
            ContentType = proxyResponse.Content.Headers.ContentType.MediaType,
            StatusCode = (int)proxyResponse.StatusCode
        };
    }

    /// <summary>
    ///     overrides the target url with the one passed from the script.
    /// </summary>
    public static void PrepareRequest(HttpClient client, HttpRequestMessage request, string url, IHeaderDictionary proxiedHeaders, string targetUrl, bool forwardToFullUrl) {
        request.RequestUri = forwardToFullUrl ? new Uri(targetUrl) : new Uri($"{targetUrl.TrimEnd('/')}/{url.TrimStart('/')}");
        C.Info($"Proxying the request to : {request.RequestUri}. Headers are");
        request.Headers.Clear();
        foreach (var h in proxiedHeaders) {
            if (h.Key == "Host") {
                continue;
            }

            request.Headers.TryAddWithoutValidation(h.Key, h.Value.ToArray());
        }


        request.Headers.Accept.TryParseAdd("*/*");
        foreach (var h in request.Headers) {
            var values = string.Join(",", h.Value);
            C.Info($"   {h.Key} = {values}");
        }
    }
}
