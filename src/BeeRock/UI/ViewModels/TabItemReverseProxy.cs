using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.LoadServiceRuleSets;
using BeeRock.Core.UseCases.StartReverseProxy;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class TabItemReverseProxy : ViewModelBase, ITabItem {
    private readonly IDocProxyRouteRepo _repo;
    private ServiceCommands _serverCommands;

    public TabItemReverseProxy(IDocProxyRouteRepo repo) {
        _repo = repo;
        CloseCommand = ReactiveCommand.Create(OnClose);
    }

    public ObservableCollection<ProxyRouteItem> ProxyRoutes { get; set; }

    public ServiceCommands ServiceCommands {
        get => _serverCommands;
        set => this.RaiseAndSetIfChanged(ref _serverCommands, value);
    }

    public MainWindowViewModel Main { get; init; }

    public string Name { get; set; }
    public ICommand CloseCommand { get; }
    public string TabType { get; } = "ReverseProxyTab";
    public string HeaderText { get; } = "Gateway";
    public bool IsServiceHost { get; } = false;

    public async Task Init() {
        ProxyRoute GetDefaultProxyRoute() {
            var d = new ProxyRoute {
                IsEnabled = true,
                LastUpdated = DateTime.Now,
                From = new ProxyRoutePart {
                    Host = "localhost:9999",
                    Scheme = "http",
                    PathTemplate = "{all}"
                },
                To = new ProxyRoutePart {
                    Host = "scl-apigateway.cxos.tech",
                    Scheme = "https",
                    PathTemplate = "{all}"
                }
            };
            return d;
        }

        var uc = new LoadProxyRouteUseCase(_repo);
        await uc.GetAll()
            .IfSucc(proxyRules => {
                if (proxyRules.IsEmpty()) {
                    proxyRules.Add(GetDefaultProxyRoute());
                    proxyRules.Add(GetDefaultProxyRoute());
                    proxyRules.Add(GetDefaultProxyRoute());
                }

                proxyRules
                    .Select(s => new ProxyRouteItem(s, _repo, RemoveService))
                    .Void(s => ProxyRoutes = new ObservableCollection<ProxyRouteItem>(s));
            });

        //auto-start the reverse proxy
        var startUc = new StartReverseProxyUseCase();
        await startUc.Start(new RestServiceSettings { Enabled = true, PortNumber = 9999 }).Match(
            CreateServiceCommands,
            exc => { C.Error(exc.ToString()); }
        );
    }

    private void RemoveService(ProxyRouteItem item) {
        Dispatcher.UIThread.InvokeAsync(() => { ProxyRoutes.Remove(item); });
    }

    public void CreateServiceCommands(IServerHostingService host) {
        ServiceCommands = new ServiceCommands(host);
    }

    private void OnClose() {
        Main.TabItems.Remove(this);
    }
}
