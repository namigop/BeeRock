namespace BeeRock.Ports.Repository;

public class RouteRuleSetsDao {
    public string HttpMethod { get; set; }
    public string RouteTemplate { get; set; }
    public string[] RuleSetIds { get; set; }
    public string MethodName { get; set; }
}
