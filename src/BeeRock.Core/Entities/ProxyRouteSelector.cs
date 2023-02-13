using System.Text;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;

namespace BeeRock.Core.Entities;

public class ProxyRouteSelector : IProxyRouteSelector2 {
    public ProxyRouteSelector(Func<ProxyRoute[]> getRoutingFilters) {
        GetRoutingFilters = getRoutingFilters;
    }

    public Func<ProxyRoute[]> GetRoutingFilters { get; }

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

    public Uri BuildUri(Uri source) {
        Dictionary<string, string> routeParameters = new();
        ProxyRoute route = null;
        foreach (var filter in this.GetRoutingFilters().Where(t => t.IsEnabled)) {
            Validate(filter);
            var (match,names) = ProxyRouteChecker.Match(source, filter);
            if (match.Success) {
                C.Info($"Route match found! : {filter.From.Scheme}://{filter.From.Host}/{filter.From.PathTemplate}");
                names.Iter(n => routeParameters[$"{{{n}}}"] = match.Groups[n].Value);
                route = filter;
                break;
            }
        }

        if (route == null) {
            throw new Exception($"Unable to match a route to the request {source.AbsoluteUri}");
        }
        else {
            var sb = new StringBuilder(route.To.PathTemplate);
            foreach (var (key, value) in routeParameters) {
                sb.Replace(key, value);
            }

            var path = sb.ToString().TrimStart('/');
            return new Uri($"{route.To.Scheme}://{route.To.Host}/{path}");
        }
    }
}
