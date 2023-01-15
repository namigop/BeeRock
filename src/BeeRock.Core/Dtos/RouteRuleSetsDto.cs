using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class RouteRuleSetsDto : IDto {
    public string HttpMethod { get; set; }
    public string MethodName { get; set; }
    public string RouteTemplate { get; set; }
    public string[] RuleSetIds { get; set; }
}