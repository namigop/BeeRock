namespace BeeRock.Core.Entities;

public class RestServiceSettings {
    public bool Enabled { get; set; } = true;
    public int PortNumber { get; set; } = 7001;

    public string SourceSwaggerDoc { get; set; } = "";
}
