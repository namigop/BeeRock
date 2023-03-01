using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using BeeRock.Core.Entities.Scripting;
using Microsoft.AspNetCore.Http;

namespace BeeRock.Core.Entities;

public class DynamicRequestHandler {
    public void Handle(HttpContext httpContext, Uri targetUri) {
        void Run(List<string> ignores) {
            var mRoute = FindMatchingMethod(httpContext.Request.Method, targetUri, ignores);
            var variables = CreateScriptVariables(httpContext, mRoute);
            var bee = new ScriptingVarBee("", mRoute.Method.MethodName, new ReadOnlyDictionary<string, object>(variables));
            variables.Add(ScriptingVarBee.VarName, bee);

            //Mak this is PassThrough so that the responses gets added directly to the Context.Items
            bee.Context.Response.SetAsPassThrough();

            _ = RequestHandler.Handle2(mRoute.Method.MethodName, variables, false);

            //If the request has not been processed, try the other routes.  This will be useful for mocking
            //soap services where you can have multiple POST routes and where route has a rule filter for the
            //SOAPAction header.
            if (!bee.Context.ContainsPassThroughResponse) {
                ignores.Add(mRoute.Method.MethodName);
                Run(ignores);
            }
        }

        var empty = new List<string>();
        Run(empty);
    }

    private Dictionary<string, object> CreateScriptVariables(HttpContext httpContext, MatchedRoute matchedRoute) {
        var variables = new Dictionary<string, object>();
        foreach (var r in matchedRoute.RouteParams) {
            var v = matchedRoute.Match.Groups[r].Value;
            variables.Add(r, v);
        }

        variables.Add(RequestHandler.ContextKey, httpContext);
        return variables;
    }

    private MatchedRoute FindMatchingMethod(string requestMethod, Uri uri, List<string> ignores) {
        var svc = RequestHandler.TestArgsProvider.FindService(uri.Port);
        foreach (var m in svc.Methods) {
            if (m.HttpMethod != requestMethod)
                continue;

            if (ignores.Contains(m.MethodName))
                continue;

            //route template matching
            var path = uri.AbsolutePath; //Ex /v1/pet/123
            var routeTemplate = m.RouteTemplate.StartsWith("/") ? m.RouteTemplate : $"/{m.RouteTemplate}";
            var pathItems = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var routeTemplateItems = routeTemplate.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathItems.Length != routeTemplateItems.Length)
                continue;

            var (match, routeParams) = RouteChecker.Match(uri, routeTemplate);
            if (match.Success)
                return new MatchedRoute {
                    Method = m,
                    Match = match,
                    RouteParams = routeParams
                };
        }

        throw new Exception($"Unable to match a route to the request HTTP {requestMethod} {uri.AbsoluteUri}");
    }

    private record MatchedRoute {
        public RestMethodInfo Method { get; init; }
        public Match Match { get; init; }
        public string[] RouteParams { get; init; }
    }
}
