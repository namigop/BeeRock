using System.Reflection;
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

        p.Add(new ParamInfo {
            Name = RequestHandler.HeaderKey,
            Type = typeof(Dictionary<string, object>),
            TypeName = "Http headers",
            DisplayValue = "Key : Value"
        });

        p.Add(new ParamInfo {
            Name = RequestHandler.FileRespKey,
            Type = typeof(ScriptingFileResponse),
            TypeName = "File Response",
            DisplayValue = @"
Use this to return a file in the http response:

1. return a csv : fileResp.ToCsv(""/path/to/my/file.csv"")
2. return an image : fileResp.ToPng(""/path/to/my/file.png"")
3. return an image : fileResp.ToJpeg(""/path/to/my/file.jpeg"")
4. return a PDF : fileResp.ToPdf(""/path/to/my/file.pdf"")
5. return any file : fileResp.ToAny(""/path/to/my/file"", ""contentType"")

"
        });

        return p;
    }
}
