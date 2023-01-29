using System.Collections.ObjectModel;
using System.Reflection;
using BeeRock.Core.Entities.CodeGen;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Http;

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

    public string ServerMethod { get; }
    public string SwaggerJson { get; init; }

    public string ForwardToServer(string baseUrl) {
        return ForwardToInternal(baseUrl, false);
    }

    private string ForwardToInternal(string url, bool isForwardingToFull) {
        Requires.NotNullOrEmpty(url, nameof(url));
        Requires.IsTrue(() => url.StartsWith("http"), nameof(url), $"{url} is not a valid URL");

        var clientType = GetClientType(SwaggerJson);
        var mi = FindMethod(ServerMethod, clientType, _variables);

        dynamic client = Activator.CreateInstance(clientType);
        client.Headers = ((ScriptingHttpHeader)_variables["header"]).Request.Headers;
        client.TargetUrl = url;
        client.IsForwardingToFullUrl = isForwardingToFull;

        var args = BuildArgs(mi, _variables);
        var respObject = mi.Invoke(client, args).ConfigureAwait(false).GetAwaiter().GetResult();
        return mi.ReturnParameter == null ? "{}" : (string)Helper.Serialize(respObject, mi.ReturnType);
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

    private static object[] BuildArgs(MethodInfo mi, ReadOnlyDictionary<string, object> variables) {
        var parameters = mi.GetParameters();
        var methodParameters = parameters.Select(pi => {
            return variables[pi.Name]
                .Then(obj => Helper.Serialize(obj, obj.GetType()))
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
                    var containsSameParameters = parameters?
                        .Select(p => variables.ContainsKey(p.Name))
                        .Aggregate((acc, i) => acc & i);
                    if ((bool)containsSameParameters)
                        return m;
                }
            }
        }

        throw new Exception($"Unable to find matching client method for {serverMethod}");
    }


    /// <summary>
    ///     Checks the proxied call status code and raises the RestHttpException (for 4xx and 5xx)
    /// </summary>
    public static void ProcessResponse(HttpClient client, HttpResponseMessage response) {
        if ((int)response.StatusCode >= 400)
            //re-throw it for the middleware to handle
            throw new RestHttpException {
                Error = response.Content != null ?
                    response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()
                    : null,
                StatusCode = response.StatusCode
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

        if (request.Headers.Accept != null) {
            request.Headers.Accept.TryParseAdd("*/*");
        }

        //request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse(""application/json""));
        foreach (var h in request.Headers) {
            var values = string.Join(",", h.Value);
            C.Info($"   {h.Key} = {values}");
        }
    }
}
