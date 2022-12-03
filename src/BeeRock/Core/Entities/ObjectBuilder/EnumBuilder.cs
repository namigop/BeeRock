namespace BeeRock.Models;

public class EnumBuilder : ITypeBuilder{
    
    public (bool, object) Build(Type type, int counter) {
        if (type.IsEnum) {
            var r = Enum.GetNames(type);
            return (true, Enum.Parse(type, r[0]));
        }
        return (false, null);
    }
}
