using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ParamInfoItem : ReactiveObject {
    public string Name { get; init; }
    public string Type { get; init; }

    public string Display => $"{Name} : {Type}";
}