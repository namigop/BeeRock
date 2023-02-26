using System.Text.RegularExpressions;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

public static class RouteChecker {
    public static (Match, string[]) Match(Uri uri, ProxyRoute condition) {
        Requires.NotNull(condition, nameof(condition));
        return Match(uri, condition.From.PathTemplate);
    }

    public static (Match, string[]) Match(Uri uri, string routeTemplate) {
        Requires.NotNullOrEmpty(routeTemplate, nameof(routeTemplate));

        var (regex, names) = ConvertToRegex(routeTemplate);
        var m = Regex.Match(uri.PathAndQuery.TrimStart('/'), regex);
        return (m, names);
    }

    public static (string, string[]) ConvertToRegex(string pathTemplate) {
        Requires.NotNullOrEmpty(pathTemplate, nameof(pathTemplate));

        //sample template: {version}/store/{id}
        var parts = pathTemplate.Split("/", StringSplitOptions.RemoveEmptyEntries);
        var allNames = new List<string>();
        if (parts.Length >= 1)
            for (var i = 0; i < parts.Length; i++) {
                var part = parts[i].Trim();
                if (part.StartsWith('{') && part.EndsWith('}') && part.Length > 2) {
                    //convert to a named regex
                    var name = part.Substring(1, part.Length - 2).Replace(" ", $"A{i}");
                    if (allNames.Contains(name))
                        throw new Exception($"Routing failed. \"{name}\" is duplicated in the path template");

                    allNames.Add(name);
                    part = $"(?<{name}>.*)";
                    parts[i] = part;
                }
            }

        return (string.Join('/', parts), allNames.ToArray());
    }
}
