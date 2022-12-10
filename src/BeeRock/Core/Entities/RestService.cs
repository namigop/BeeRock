using BeeRock.Adapters;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public class RestService : IRestService {
    private readonly RestServiceSettings _settings;

    public RestService(Type[] controllerTypes, string name, RestServiceSettings settings) {
        Methods = controllerTypes.SelectMany(c => new RestControllerReader().Inspect(c)).ToList();
        ControllerTypes = controllerTypes;
        Name = name;
        Settings = settings;
    }

    public Type[] ControllerTypes { get; init; }

    public List<RestMethodInfo> Methods { get; }

    public string Name { get; init; }
    public string SwaggerUrl { get; private set; }


    public RestServiceSettings Settings {
        get => _settings;
        init {
            _settings = value;
            SwaggerUrl = $"http://localhost:{_settings.PortNumber}/swagger/index.html";
        }
    }
}