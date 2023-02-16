using System.Reflection;
using BeeRock.Core.Entities.Scripting;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BeeRock.Core.Entities;

public class RestControllerReader : IRestControllerReader {
    /// <summary>
    ///     Read the metadata info from the controller and figure out the rest endpoints
    /// </summary>
    public List<RestMethodInfo> Inspect(Type controllerType) {
        Requires.NotNull(controllerType, nameof(controllerType));

        var classRouteAttr2 = controllerType.GetCustomAttributes(typeof(RouteAttribute)).FirstOrDefault();
        var classRouteTemplate = "";
        if (classRouteAttr2 is RouteAttribute x) classRouteTemplate = x.Template;

        var methods = controllerType.GetMethods().Where(mi => mi.GetCustomAttributes(false).Any());
        return methods.Select(m => GetMethodInformation(m, classRouteTemplate))
            .Where(m => m != null)
            .ToList();
    }

    /// <summary>
    ///     Use reflection to get details on the methods
    /// </summary>
    private RestMethodInfo GetMethodInformation(MethodInfo methodInfo, string controllerRouteTemplate) {
        Requires.NotNull(methodInfo, nameof(methodInfo));

        var obs = methodInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).FirstOrDefault();
        var r = methodInfo.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
        var methodAttr = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute), true).FirstOrDefault();
        if (r is RouteAttribute routeAttr && methodAttr is HttpMethodAttribute mAttr) {
            var httpMethod = mAttr.HttpMethods.First();
            var methodName = methodInfo.Name;
            var template = $"{controllerRouteTemplate}/{routeAttr.Template}";

            if (methodInfo.ReturnType.IsGenericType) {
                var genericTypeArg = methodInfo.ReturnType.GetGenericArguments().First();

                //Console.WriteLine($"{httpMethod} {template}  {methodName} -> {genericTypeArg.Name}");
                return new RestMethodInfo {
                    HttpMethod = httpMethod,
                    MethodName = methodName,
                    ReturnType = genericTypeArg,
                    RouteTemplate = template,
                    Parameters = GetParams(methodInfo),
                    Rules = new List<Rule>(),
                    IsObsolete = obs != null
                };
            }

            //Console.WriteLine($"{httpMethod} {template} {methodName} -> void");
            return new RestMethodInfo {
                HttpMethod = httpMethod,
                MethodName = methodName,
                ReturnType = typeof(void),
                RouteTemplate = template,
                Parameters = GetParams(methodInfo),
                Rules = new List<Rule>(),
                IsObsolete = obs != null
            };
        }

        return null;
    }

    private List<ParamInfo> GetParams(MethodInfo methodInfo) {
        static string FormatTypeName(Type type) {
            if (type.IsGenericType) {
                var args =
                    type.GetGenericArguments().Select(t => t.Name)
                        .Then(t => string.Join(",", t));
                return type.Name.Split("`").First()
                    .Then(n => $"{n}<{args}>");
            }

            return type.Name;
        }

        Requires.NotNull(methodInfo, nameof(methodInfo));
        var p = methodInfo.GetParameters()
            .Select(p => new ParamInfo {
                TypeName = FormatTypeName(p.ParameterType),
                Type = p.ParameterType,
                Name = p.Name,
                DisplayValue = null
            })
            .ToList();

        p.Add(ScriptingVarUtils.GetHeadersParamInfo());
        p.Add(ScriptingVarUtils.GetQueryStringParamInfo());
        p.Add(ScriptingVarUtils.GetFileRespParamInfo());
        p.Add(ScriptingVarUtils.GetRunParamInfo());
        p.Add(ScriptingVarUtils.GetRmqParamInfo());
        p.Add(ScriptingVarUtils.GetProxyParamInfo());
        p.Add(ScriptingVarUtils.GetContextParamInfo());
        p.Add(ScriptingVarUtils.GetLogParamInfo());
        return p;
    }
}
