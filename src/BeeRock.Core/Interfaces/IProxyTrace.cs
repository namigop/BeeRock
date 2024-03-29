using System.Net;

namespace BeeRock.Core.Interfaces;

public interface IProxyTrace {
    //public long Id { get; set; }
    public uint ElapsedMsec { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; }
    public Uri RequestUri { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string RequestMethod { get; set; }
}
