namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceSelection {
    public string Name { get; set; }
    public string SwaggerUrlOrFile { get; set; }
    public int PortNumber { get; set; }
    public string DocId { get; set; }

    public string Display => $"{Name} : {SwaggerUrlOrFile}";
}
