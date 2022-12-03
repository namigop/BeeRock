using BeeRock.APP.Services;
using BeeRock.Core.Entities;
using BeeRock.ViewModels;
using ReactiveUI;

namespace BeeRock.Models;

public class ServiceItem : ViewModelBase {
    private string name = "";
    private string swaggerUrl = "";
    private string url = "";
    private Settings settings;
    private string searchText;

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

    private void FilterMethods(string text) {
        if (string.IsNullOrWhiteSpace(text)) {
            foreach (var m in Methods)
                m.CanShow = true;
        }
        else {
            foreach (var m in Methods) {
                m.CanShow = m.Method.RouteTemplate.Contains(text);
            }
        }
    }

    public List<ServiceMethodItem> Methods { get; private set; }

    public string Name {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public string SwaggerUrl {
        get => swaggerUrl;
        set => this.RaiseAndSetIfChanged(ref swaggerUrl, value);
    }

    public string Url {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    public Settings Settings {
        get => settings;
        set {
            this.RaiseAndSetIfChanged(ref settings, value);
            SwaggerUrl = $"http://localhost:{settings.PortNumber}/swagger/index.html";
        }
    }

    public string SearchText {
        get => searchText;
        set => this.RaiseAndSetIfChanged( ref searchText , value);
    }
}
