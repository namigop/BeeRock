using BeeRock.Core.Interfaces;

namespace BeeRock.Repository;

public class WhenDao : IDao {
    public string BooleanExpression { get; set; }
    public bool IsActive { get; set; }
}