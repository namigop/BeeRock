using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IRestService : IDoc {
    Type[] ControllerTypes { get; init; }
    List<RestMethodInfo> Methods { get; }
    string Name { get; init; }
    RestServiceSettings Settings { get; init; }
    string SwaggerUrl { get; }
}