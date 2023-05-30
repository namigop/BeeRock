using System.Text;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

public class ProxyRouteSelector : IProxyRouteSelector {
    public ProxyRouteSelector(Func<ProxyRoute[]> getRoutingFilters) {
        GetRoutingFilters = getRoutingFilters;
    }

    public Func<ProxyRoute[]> GetRoutingFilters { get; }

    public (ProxyRoute, Dictionary<string, string>) FindMatchingRoute(Uri source) {
        Dictionary<string, string> routeParameters = new();
        ProxyRoute routeConfig = null;
        foreach (var filter in GetRoutingFilters().Where(t => t.IsEnabled)) {
            Validate(filter);
            var (match, names) = RouteChecker.Match(source, filter);
            if (match.Success) {
                C.Info($"Route match found! : {filter.From.Scheme}://{filter.From.Host}/{filter.From.PathTemplate}");
                _ = names.Iter(n => routeParameters[$"{{{n}}}"] = match.Groups[n].Value);
                routeConfig = filter;
                break;
            }
        }

        if (routeConfig == null) throw new Exception($"Unable to match a route to the request {source.AbsoluteUri}");

        return (routeConfig, routeParameters);
    }

    public Uri BuildUri(Uri source, ProxyRoute routeConfig, Dictionary<string, string> routeParameters) {
        this.SelectedRouteConfig = routeConfig;
        var sb = new StringBuilder(routeConfig.To.PathTemplate);
        foreach (var (key, value) in routeParameters) 
            _ = sb.Replace(key, value);

        var path = sb.ToString().TrimStart('/');
        var uri = new Uri($"{routeConfig.To.Scheme}://{routeConfig.To.Host}/{path}");

        // if the source URI has query params, add it only if our newly built URI does not have it
        if (!string.IsNullOrWhiteSpace(source.Query) && string.IsNullOrWhiteSpace(uri.Query) ) {
            return new Uri( uri.AbsoluteUri + source.Query);
        }

        return uri;
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
