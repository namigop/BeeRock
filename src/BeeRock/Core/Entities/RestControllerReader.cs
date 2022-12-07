using System.Reflection;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BeeRock.Core.Entities;

public class RestControllerReader {
    public RestMethodInfo GetMethodInformation(MethodInfo m) {
        var data = m.GetCustomAttributesData();
        var r = m.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
        var methodAttr = m.GetCustomAttributes(typeof(HttpMethodAttribute), true).FirstOrDefault();
        if (r is RouteAttribute routeAttr && methodAttr is HttpMethodAttribute mAttr) {
            var httpMethod = mAttr.HttpMethods.First();
            var methodName = m.Name;
            var retIsTask = m.ReturnType.IsAssignableTo(typeof(Task));
            var template = routeAttr.Template ?? "";
            if (m.ReturnType.IsGenericType) {
                var genericTypeArg = m.ReturnType.GetGenericArguments().First();

                //Console.WriteLine($"{httpMethod} {template}  {methodName} -> {genericTypeArg.Name}");

                return new RestMethodInfo {
                    HttpMethod = httpMethod,
                    MethodName = methodName,
                    ReturnType = genericTypeArg,
                    RouteTemplate = template,
                    Parameters = GetParams(m)
                };
            }

            //Console.WriteLine($"{httpMethod} {template} {methodName} -> void");
            return new RestMethodInfo {
                HttpMethod = httpMethod,
                MethodName = methodName,
                ReturnType = typeof(void),
                RouteTemplate = template,
                Parameters = GetParams(m)
            };
        }

        return null;
    }

    private List<ParamInfo> GetParams(MethodInfo methodInfo) {
        static string FormatTypeName(Type type) {
            if (type.IsGenericType) {
                var args =
                    type.GetGenericArguments().Select(t => t.Name.ToLower())
                        .Then(t => string.Join(",", t));
                return type.Name.Split("`").First()
                    .Then(n => $"{n}<{args}>");
            }

            return type.Name;
        }

        return methodInfo.GetParameters()
            .Select(p => new ParamInfo { Type = FormatTypeName(p.ParameterType).ToLower(), Name = p.Name })
            .ToList();
    }

    public List<RestMethodInfo> Inspect(Type controllerType) {
        var methods = controllerType.GetMethods().Where(mi => mi.GetCustomAttributes(false).Any());
        return methods.Select(m => GetMethodInformation(m))
            .Where(m => m != null)
            .ToList();
    }
}
