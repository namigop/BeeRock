namespace BeeRock.Core.Entities;

public record ProxyRoutePart {
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string PathTemplate { get; set; }
}
