using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public record DocReqRespTraceDto : IDoc, IDto
{
    public uint ElapsedMsec { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; }
    public string RequestUri { get; set; }
    public string StatusCode { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; }
    public string RequestMethod { get; set; }
}
