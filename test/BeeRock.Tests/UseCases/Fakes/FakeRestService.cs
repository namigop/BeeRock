using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

namespace BeeRock.Tests.UseCases.Fakes;

public class FakeRestService : ICompiledRestService {
    public Type[] ControllerTypes { get; init; } = Array.Empty<Type>();
    public List<RestMethodInfo> Methods { get; init; } = new();
    public string Name { get; init; } = "";
    public string SwaggerUrl { get; init; } = "";
    public RestServiceSettings Settings { get; init; } = new();
    public string DocId { get; set; } = Guid.NewGuid().ToString();
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}
