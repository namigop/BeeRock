namespace BeeRock.Core.Entities;

public record ParamInfo {
    public string Name { get; init; }
    public string TypeName { get; init; }
    public Type Type { get; init; }
}
