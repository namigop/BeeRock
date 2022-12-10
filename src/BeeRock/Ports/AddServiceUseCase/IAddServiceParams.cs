namespace BeeRock.Ports.AddServiceUseCase;

public interface IAddServiceParams {
    string SwaggerUrl { get; init; }
    string ServiceName { get; init; }
    int Port { get; init; }
}