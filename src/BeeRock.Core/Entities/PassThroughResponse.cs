namespace BeeRock.Core.Entities;

public class PassThroughResponse {
    public string ContentType { get; init; }
    public string Content { get; init; }
    public int StatusCode { get; init; }
}
