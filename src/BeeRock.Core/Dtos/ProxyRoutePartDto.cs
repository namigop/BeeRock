namespace BeeRock.Core.Dtos;

public record ProxyRoutePartDto {
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string PathTemplate { get; set; }
}