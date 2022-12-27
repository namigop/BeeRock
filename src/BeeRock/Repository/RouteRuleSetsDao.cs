using BeeRock.Core.Interfaces;

namespace BeeRock.Repository;

public class RouteRuleSetsDao : IDao {
    public string HttpMethod { get; set; }
    public string RouteTemplate { get; set; }
    public string[] RuleSetIds { get; set; }
    public string MethodName { get; set; }
}
