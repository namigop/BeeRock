namespace BeeRock.Core.Entities.ObjectBuilder;

public class EnumBuilder : ITypeBuilder {

    /// <summary>
    ///     Create an instance of an Enumeration type
    /// </summary>
    public (bool, object) Build(Type type, int counter) {
        if (type.IsEnum) {
            var r = Enum.GetNames(type);
            return (true, Enum.Parse(type, r[0]));
        }

        return (false, null);
    }
}