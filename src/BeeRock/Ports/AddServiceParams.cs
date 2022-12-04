namespace BeeRock.Ports;

public class AddServiceParams {
    public string SwaggerUrl { get; init; }
    public string ServiceName { get; init; }
    public int Port { get; init; }
}