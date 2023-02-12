namespace BeeRock.Core.Interfaces;

public interface IRoutingMetric {
    string HttpMethod { get; set; }
    string Uri { get; set; }
    long? RequestLength { get; set; }
    long? ResponseLength { get; set; }
    TimeSpan Elapsed { get; set; }
    int StatusCode { get; set; }
}
