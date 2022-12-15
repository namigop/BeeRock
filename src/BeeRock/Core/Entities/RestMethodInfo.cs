namespace BeeRock.Core.Entities;

public record RestMethodInfo {
    public string RouteTemplate { get; init; }
    public string MethodName { get; init; }
    public string HttpMethod { get; init; }
    public Type ReturnType { get; init; }
    public List<ParamInfo> Parameters { get; init; }

    public List<Rule> Rules { get; init; }
}
