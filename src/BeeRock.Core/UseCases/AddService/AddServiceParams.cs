namespace BeeRock.Core.UseCases.AddService;

public class AddServiceParams {
    public string DocId { get; set; }
    public int Port { get; init; }
    public string ServiceName { get; init; }
    public string SwaggerUrl { get; init; }
    public string TempPath { get; set; }
}