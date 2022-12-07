using System.Collections.ObjectModel;
using System.Reactive.Linq;
using BeeRock.Core.Entities;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public partial class ServiceItem : ViewModelBase {
    private string _name = "";
    private string _searchText;
    private RestServiceSettings _settings;
    private string _swaggerUrl = "";
    private string _url = "";

    public ServiceItem() {
        //for the designer intellisense
    }

    public ServiceItem(RestService svc) {
        Name = svc.Name;
        var m = svc.Methods.Select(r => new ServiceMethodItem(r));
        Methods = new ObservableCollection<ServiceMethodItem>(m);

        this.WhenAnyValue(t => t.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(t => FilterMethods(t));
    }

    public MainWindowViewModel Main { get; init; }
    public ObservableCollection<ServiceMethodItem> Methods { get; init; }

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

    public RestServiceSettings Settings {
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

    public async Task Close() {
        var msg = "Are you sure you want to close this tab?";
        var msBoxStandardWindow = MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                ButtonDefinitions = ButtonEnum.YesNoCancel,
                ContentTitle = "Please Confirm",
                ContentMessage = msg,
                Icon = Icon.Question
            });

        var result = await msBoxStandardWindow.Show();
        if (result == ButtonResult.Yes) {
            Main.Services.Remove(this);
            if (!Main.Services.Any()) Main.SelectedTabIndex = 0; //back to add service dialog
        }
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