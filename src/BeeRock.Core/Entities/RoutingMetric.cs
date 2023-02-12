using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public record RoutingMetric : IRoutingMetric {
    public string HttpMethod { get; set; }
    public string Uri { get; set; }
    public long? RequestLength { get; set; }
    public long? ResponseLength { get; set; }
    public TimeSpan Elapsed { get; set; }
    public int StatusCode { get; set; }
}
