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
    private readonly string _host;
    private readonly RoutingMetricCalculator _metricCalculator = new();
    private readonly IDocProxyRouteRepo _repo;
    private readonly RestServiceSettings _settings;
    private int _portNumber;
    private ProxyRouteItem _selectedProxyRoute;
    private ServiceCommands _serverCommands;

    public TabItemReverseProxy(IDocProxyRouteRepo repo) {
        _repo = repo;
        CloseCommand = ReactiveCommand.Create(OnClose);
        AddProxyRouteCommand = ReactiveCommand.Create(OnAddProxyRoute);
        MoveUpCommand = ReactiveCommand.Create(OnMoveUp);
        MoveDownCommand = ReactiveCommand.Create(OnMoveDown);
        _settings = new RestServiceSettings { Enabled = true, PortNumber = 0 };
        PortNumber = 9999;
        _host = "localhost";
        ProxyRoutes = new ObservableCollection<ProxyRouteItem>();
    }

    public ObservableCollection<ProxyRouteItem> ProxyRoutes { get; }

    public ServiceCommands ServiceCommands {
        get => _serverCommands;
        set => this.RaiseAndSetIfChanged(ref _serverCommands, value);
    }

    public MainWindowViewModel Main { get; init; }

    public ProxyMetricsViewModel MetricsViewModel { get; } = new();

    public ICommand AddProxyRouteCommand { get; }
    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }

    public ProxyRouteItem SelectedProxyRoute {
        get => _selectedProxyRoute;
        set => this.RaiseAndSetIfChanged(ref _selectedProxyRoute, value);
    }

    public int PortNumber {
        get => _portNumber;
        set {
            this.RaiseAndSetIfChanged(ref _portNumber, value);
            _settings.PortNumber = value;
        }
    }

    public string Name { get; set; }
    public ICommand CloseCommand { get; }
    public string TabType { get; } = "ReverseProxyTab";
    public string HeaderText { get; } = "Reverse Proxy";
    public bool IsServiceHost { get; } = false;

    public async Task Init() {
        ProxyRoute GetDefaultProxyRoute() {
            var d = new ProxyRoute {
                IsEnabled = true,
                LastUpdated = DateTime.Now,
                From = new ProxyRoutePart {
                    Host = $"{_host}:{PortNumber}",
                    Scheme = "http",
                    PathTemplate = "{all}"
                },
                To = new ProxyRoutePart {
                    Host = "server",
                    Scheme = "https",
                    PathTemplate = "{all}"
                }
            };
            return d;
        }

        var uc = new LoadProxyRouteUseCase(_repo);
        await uc.GetAll()
            .IfSucc(proxyRules => {
                if (proxyRules.IsEmpty()) proxyRules.Add(GetDefaultProxyRoute());

                foreach (var i in proxyRules.OrderBy(t => t.Index)) {
                    var item = new ProxyRouteItem(i, _repo, Remove);
                    ProxyRoutes.Add(item);
                }
            });

        //auto-start the reverse proxy
        var startUc = new StartReverseProxyUseCase();
        var proxyHandler = new ProxyRouteHandler(GetProxyRouteFilters, OnBeginRouting, OnEndRouting);
        await startUc.Start(_settings, proxyHandler).Match(
            CreateServiceCommands,
            exc => { C.Error(exc.ToString()); }
        );
    }

    private void OnBeginRouting(ProxyRoute selectedRoute) {
        var p = ProxyRoutes.First(p => p.Index == selectedRoute.Index);
        p.IsActive = true;
    }


    private void OnEndRouting(IRoutingMetric metric) {
        _metricCalculator.Run(metric);
        MetricsViewModel.Refresh(_metricCalculator);
        var p = ProxyRoutes.First(p => p.Index == metric.RouteIndex);
        p.IsActive = false;
    }

    private ProxyRoute[] GetProxyRouteFilters() {
        return ProxyRoutes.Select(p => p.ToRoute()).ToArray();
    }

    private void Remove(ProxyRouteItem item) {
        Dispatcher.UIThread.InvokeAsync(() => { ProxyRoutes.Remove(item); });
    }

    public void CreateServiceCommands(IServerHostingService host) {
        ServiceCommands = new ServiceCommands(host);
    }

    private void OnClose() {
        Main.TabItems.Remove(this);
    }

    private void OnAddProxyRoute() {
        var prox = new ProxyRoute {
            Index = ProxyRoutes.Count,
            IsEnabled = false, //disabled by default. user has to manually enable it
            LastUpdated = DateTime.Now,
            From = new ProxyRoutePart {
                Host = $"{_host}:{_settings.PortNumber}",
                PathTemplate = "{all}",
                Scheme = "http"
            },
            To = new ProxyRoutePart {
                Host = "server",
                PathTemplate = "{all}",
                Scheme = "https"
            }
        };

        var item = new ProxyRouteItem(prox, _repo, Remove);
        ProxyRoutes.Add(item);
        SaveAll();
    }

    private void SaveAll() {
        ProxyRoutes.Iter((index, p) => {
            p.Index = index;
            p.Save();
        });
    }


    private void OnMoveUp() {
        if (SelectedProxyRoute is { }) {
            var thisItem = SelectedProxyRoute;
            var pos = ProxyRoutes.IndexOf(thisItem);
            if (pos > 0) {
                ProxyRoutes.RemoveAt(pos);
                ProxyRoutes.Insert(pos - 1, thisItem);
                SelectedProxyRoute = thisItem;
                SaveAll();
            }
        }
    }

    private void OnMoveDown() {
        if (SelectedProxyRoute is { }) {
            var thisItem = SelectedProxyRoute;
            var pos = ProxyRoutes.IndexOf(thisItem);
            if (pos < ProxyRoutes.Count - 1) {
                ProxyRoutes.RemoveAt(pos);
                ProxyRoutes.Insert(pos + 1, thisItem);
                SelectedProxyRoute = thisItem;
                SaveAll();
            }
        }
    }
}
