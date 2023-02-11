using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Dtos;

public record WhenDto : IDto {
    public string BooleanExpression { get; set; }
    public bool IsActive { get; set; }
}
