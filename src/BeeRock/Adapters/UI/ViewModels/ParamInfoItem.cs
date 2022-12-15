using BeeRock.Core.Entities.ObjectBuilder;

using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ParamInfoItem : ReactiveObject {
    public string Name { get; init; }
    public string TypeName { get; init; }

    public string Display => $"{Name} : {TypeName}";
    public Type Type { get; init; }
    
    public string DefaultJson { get => ObjectBuilder.CreateNewInstanceAsJson(Type, 0); }
}
