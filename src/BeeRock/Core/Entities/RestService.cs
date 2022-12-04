using BeeRock.Adapters;

namespace BeeRock.Core.Entities;

public enum LogType {
    Info,
    Error,
    Warning,
    Debug
}

public record Log(LogType Type, string Message);

public class RestService {
    private RestServiceSettings _settings;

    public RestService(Type[] controllerTypes) {
        Methods = controllerTypes.SelectMany(c => new RestControllerReader().Inspect(c)).ToList();
        ControllerTypes = controllerTypes;
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
