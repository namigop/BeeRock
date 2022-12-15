namespace BeeRock.Ports.AddServiceUseCase;

public class AddServiceParams {
    public string SwaggerUrl { get; init; }
    public string ServiceName { get; init; }
    public int Port { get; init; }

    public string DocId { get; set; }
}
