using BeeRock.Adapters;
using BeeRock.Core.Entities;

namespace BeeRock.Core.Interfaces;

public interface IRestService {
    Type[] ControllerTypes { get; init; }
    List<RestMethodInfo> Methods { get; }
    string Name { get; init; }
    string SwaggerUrl { get; }
    RestServiceSettings Settings { get; init; }
    string DocId { get; set; }
}
