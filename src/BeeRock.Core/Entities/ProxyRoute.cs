using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public record ProxyRoute : IDoc {
    public ProxyRoutePart From { get; set; }
    public ProxyRoutePart To { get; set; }
    public bool IsEnabled { get; set; }

    public int Index { get; set; } = -1;
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; }
}
