using BeeRock.Core.Interfaces;

namespace BeeRock.Ports.Repository;

public class WhenDao: IDao {
    public bool IsActive { get; set; }
    public string BooleanExpression { get; set; }
}
