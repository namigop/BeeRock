namespace BeeRock.Core.Entities;

public interface IDoc {
    string DocId { get; set; }
    DateTime LastUpdated { get; set; }
}