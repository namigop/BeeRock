using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ParamInfoItem : ReactiveObject {
    public string Name { get; init; }
    public string TypeName { get; init; }

    public string Display => $"{Name} : {TypeName}";
    public Type Type { get; init; }
}
