namespace BeeRock.Core.Interfaces;

public interface IDoc {
    string DocId { get; set; }
    DateTime LastUpdated { get; set; }
}