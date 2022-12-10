using System.Reflection;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BeeRock.Core.Entities;

public class RestControllerReader : IRestControllerReader {
    public List<RestMethodInfo> Inspect(Type controllerType) {
        var methods = controllerType.GetMethods().Where(mi => mi.GetCustomAttributes(false).Any());
        return methods.Select(GetMethodInformation)
            .Where(m => m != null)
            .ToList();
    }

    private RestMethodInfo GetMethodInformation(MethodInfo methodInfo) {
        Requires.NotNull(methodInfo, nameof(methodInfo));

        var classType = methodInfo.DeclaringType;
        var classRouteAttr2 = classType.GetCustomAttributes(typeof(RouteAttribute)).FirstOrDefault();
        var classRouteTemplate = "";
        if (classRouteAttr2 is RouteAttribute classRouteAttr) classRouteTemplate = classRouteAttr.Template;

        var r = methodInfo.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
        var methodAttr = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute), true).FirstOrDefault();
        if (r is RouteAttribute routeAttr && methodAttr is HttpMethodAttribute mAttr) {
            var httpMethod = mAttr.HttpMethods.First();
            var methodName = methodInfo.Name;
            var template = $"{classRouteTemplate}/{routeAttr.Template}";

            if (methodInfo.ReturnType.IsGenericType) {
                var genericTypeArg = methodInfo.ReturnType.GetGenericArguments().First();

                //Console.WriteLine($"{httpMethod} {template}  {methodName} -> {genericTypeArg.Name}");
                return new RestMethodInfo {
                    HttpMethod = httpMethod,
                    MethodName = methodName,
                    ReturnType = genericTypeArg,
                    RouteTemplate = template,
                    Parameters = GetParams(methodInfo)
                };
            }

            //Console.WriteLine($"{httpMethod} {template} {methodName} -> void");
            return new RestMethodInfo {
                HttpMethod = httpMethod,
                MethodName = methodName,
                ReturnType = typeof(void),
                RouteTemplate = template,
                Parameters = GetParams(methodInfo)
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

        return methodInfo.GetParameters()
            .Select(p => new ParamInfo { Type = FormatTypeName(p.ParameterType), Name = p.Name })
            .ToList();
    }
}
