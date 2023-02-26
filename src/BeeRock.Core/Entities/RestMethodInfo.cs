namespace BeeRock.Core.Entities;

public record RestMethodInfo {
    public string RouteTemplate { get; set; }
    public string MethodName { get; init; }
    public string HttpMethod { get; set; }
    public Type ReturnType { get; init; }
    public List<ParamInfo> Parameters { get; init; }

    public List<Rule> Rules { get; init; }
    public bool IsObsolete { get; init; }
}
