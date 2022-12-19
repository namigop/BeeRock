using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace BeeRock.Adapters.UI.ViewModels;

public class TabItemSelector : IDataTemplate {


    [Content]
    public Dictionary<string, IDataTemplate> Templates {get;} = new();

    public IControl Build(object data)
    {
        return Templates[((ITabItem) data).TabType].Build(data);
    }

    public bool Match(object data)
    {
        return data is ITabItem;
    }

}
