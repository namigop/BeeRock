using System.Reflection;
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

                Console.WriteLine($"{httpMethod} {template}  {methodName} -> {genericTypeArg.Name}");

                return new RestMethodInfo {
                    HttpMethod = httpMethod,
                    MethodName = methodName,
                    ReturnType = genericTypeArg,
                    RouteTemplate = template
                };
            }

            Console.WriteLine($"{httpMethod} {template} {methodName} -> void");
            return new RestMethodInfo {
                HttpMethod = httpMethod,
                MethodName = methodName,
                ReturnType = typeof(void),
                RouteTemplate = template
            };
        }

        return null;
    }

    public List<RestMethodInfo> Inspect(Type controllerType) {
        var methods = controllerType.GetMethods().Where(mi => mi.GetCustomAttributes(false).Any());
        return methods.Select(m => GetMethodInformation(m))
            .Where(m => m != null)
            .ToList();
    }
}
