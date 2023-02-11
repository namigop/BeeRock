namespace BeeRock.Repository;

public record ProxyRoutePartDao {
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string PathTemplate { get; set; }
}
