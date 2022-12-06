namespace BeeRock.Core.Entities;

public class RestMethodInfo {
    public string RouteTemplate { get; init; }
    public string MethodName { get; init; }
    public string HttpMethod { get; init; }
    public Type ReturnType { get; init; }
}