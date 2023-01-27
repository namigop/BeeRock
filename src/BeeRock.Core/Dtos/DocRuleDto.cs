using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class DocRuleDto : IDoc, IDto {
    public string Body { get; set; }
    public WhenDto[] Conditions { get; set; }
    public int DelayMsec { get; set; }
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int StatusCode { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
}