using System.Text;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

public class ProxyRouteSelector : IProxyRouteSelector {
    public ProxyRouteSelector(Func<ProxyRoute[]> getRoutingFilters) {
        GetRoutingFilters = getRoutingFilters;
    }

    public Func<ProxyRoute[]> GetRoutingFilters { get; }

    public Uri BuildUri(Uri source) {
        Dictionary<string, string> routeParameters = new();
        ProxyRoute routeConfig = null;
        foreach (var filter in GetRoutingFilters().Where(t => t.IsEnabled)) {
            Validate(filter);
            var (match, names) = RouteChecker.Match(source, filter);
            if (match.Success) {
                C.Info($"Route match found! : {filter.From.Scheme}://{filter.From.Host}/{filter.From.PathTemplate}");
                names.Iter(n => routeParameters[$"{{{n}}}"] = match.Groups[n].Value);
                routeConfig = filter;
                break;
            }
        }

        SelectedRouteConfig = routeConfig;

        if (routeConfig == null) throw new Exception($"Unable to match a route to the request {source.AbsoluteUri}");

        var sb = new StringBuilder(routeConfig.To.PathTemplate);
        foreach (var (key, value) in routeParameters) sb.Replace(key, value);

        var path = sb.ToString().TrimStart('/');
        var uriPath = $"{routeConfig.To.Scheme}://{routeConfig.To.Host}/{path}";
        if (!string.IsNullOrWhiteSpace(source.Query)) return new Uri(uriPath + source.Query);

        return new Uri(uriPath);
    }

    public ProxyRoute SelectedRouteConfig { get; private set; }

    private static void Validate(ProxyRoute condition) {
        Requires.NotNull(condition, nameof(condition));
        Requires.NotNull(condition.From, nameof(condition.From));
        Requires.NotNullOrEmpty(condition.From.PathTemplate, nameof(condition.From.PathTemplate));
        Requires.NotNullOrEmpty(condition.From.Host, nameof(condition.From.Host));
        Requires.NotNullOrEmpty(condition.From.Scheme, nameof(condition.From.Scheme));

        Requires.NotNull(condition.To, nameof(condition.To));
        Requires.NotNullOrEmpty(condition.To.PathTemplate, nameof(condition.To.PathTemplate));
        Requires.NotNullOrEmpty(condition.To.Host, nameof(condition.To.Host));
        Requires.NotNullOrEmpty(condition.To.Scheme, nameof(condition.To.Scheme));
    }
}
