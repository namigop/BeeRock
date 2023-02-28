namespace BeeRock.UI.ViewModels;

public class ServiceSelection {

    public ServiceSelection(){}
    public string Name { get; set; }
    public string SwaggerUrlOrFile { get; set; }
    public int PortNumber { get; set; }
    public string DocId { get; set; }

    public string Display => IsDynamic ? "<Dynamic mock service>" : $"{SwaggerUrlOrFile}";
    public bool IsDynamic { get; set; }
}
