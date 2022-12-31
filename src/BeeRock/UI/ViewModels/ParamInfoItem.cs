using BeeRock.Core.Entities.ObjectBuilder;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ParamInfoItem : ReactiveObject {
    private readonly Type _type;
    private string _defaultJson;
    public string Name { get; init; }
    public string TypeName { get; init; }

    public string Display => $"{Name} : {TypeName}";

    public Type Type {
        get => _type;
        init {
            _type = value;

        }
    }

    public string DefaultJson {
        get => _defaultJson;
        set => this.RaiseAndSetIfChanged(ref _defaultJson, value);
    }
}
