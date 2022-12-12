namespace BeeRock.Ports.Repository;

public class DocServiceRuleSetsDao {
    public string ServiceName { get; set; }
    public int PortNumber { get; set; }
    public string SourceSwagger { get; set; }
    public RouteRuleSetsDao[] Routes { get; set; }
    public string DocId { get; set; }
}