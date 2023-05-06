using System.Net;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public record ProxyTrace : IProxyTrace
{
    public long Id { get; set; }
    public uint ElapsedMsec { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, object> RequestHeaders { get; set; }
    public Dictionary<string, object> ResponseHeaders { get; set; }
    public Uri RequestUri { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
