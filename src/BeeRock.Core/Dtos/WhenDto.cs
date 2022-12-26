using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class WhenDto : IDto {
    public bool IsActive { get; set; }
    public string BooleanExpression { get; set; }
}
