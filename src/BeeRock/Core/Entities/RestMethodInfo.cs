namespace BeeRock.Core.Entities;

public class RestMethodInfo {
    public string RouteTemplate { get; set; }
    public string MethodName { get; set; }
    public string HttpMethod { get; set; }
    public Type ReturnType { get; set; }
}
