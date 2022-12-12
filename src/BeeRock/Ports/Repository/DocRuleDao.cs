namespace BeeRock.Ports.Repository;

public class DocRuleDao {
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public WhenDao[] Conditions { get; set; }
    public string DocId { get; set; }
}
