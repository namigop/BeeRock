namespace BeeRock.Ports.AddServiceUseCase;

public class AddServiceParams : IAddServiceParams {
    public string SwaggerUrl { get; init; }
    public string ServiceName { get; init; }
    public int Port { get; init; }
}