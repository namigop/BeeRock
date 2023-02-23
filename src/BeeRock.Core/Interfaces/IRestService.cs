using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IRestService : IDoc {

    List<RestMethodInfo> Methods { get; }
    string Name { get; init; }
    RestServiceSettings Settings { get; init; }
    string SwaggerUrl { get; }
}

public interface ICompiledRestService : IRestService {
    Type[] ControllerTypes { get; init; }
}
