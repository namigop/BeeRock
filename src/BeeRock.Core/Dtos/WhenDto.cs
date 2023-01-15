using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public class WhenDto : IDto {
    public string BooleanExpression { get; set; }
    public bool IsActive { get; set; }
}