using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Entities;
using ReactiveUI;

namespace BeeRock.Adapters.UI.Models;

public class ServiceItem : ViewModelBase {
    private string _name = "";
    private string _searchText;
    private Settings _settings;
    private string _swaggerUrl = "";
    private string _url = "";

    public ServiceItem(Type controllerType) {
        var reader = new RestControllerReader();
        Methods =
            reader.Inspect(controllerType)
                .OrderBy(r => r.RouteTemplate)
                .Select(m => new ServiceMethodItem(m))
                .ToList();

        this.WhenAnyValue(t => t.SearchText)
            .Subscribe(t => FilterMethods(t));
    }

    public List<ServiceMethodItem> Methods { get; }

    public string Name {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string SwaggerUrl {
        get => _swaggerUrl;
        set => this.RaiseAndSetIfChanged(ref _swaggerUrl, value);
    }

    public string Url {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public Settings Settings {
        get => _settings;
        set {
            this.RaiseAndSetIfChanged(ref _settings, value);
            SwaggerUrl = $"http://localhost:{_settings.PortNumber}/swagger/index.html";
        }
    }

    public string SearchText {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private void FilterMethods(string text) {
        if (string.IsNullOrWhiteSpace(text))
            foreach (var m in Methods)
                m.CanShow = true;
        else
            foreach (var m in Methods)
                m.CanShow = m.Method.RouteTemplate.Contains(text);
    }
}
