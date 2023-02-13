using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Avalonia.Threading;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.LoadServiceRuleSets;
using BeeRock.Core.UseCases.SaveRouteRule;
using BeeRock.Core.UseCases.StartReverseProxy;
using BeeRock.Core.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class TabItemReverseProxy : ViewModelBase, ITabItem {
    private readonly IDocProxyRouteRepo _repo;
    private ServiceCommands _serverCommands;
    private readonly RestServiceSettings _settings;
    private readonly string _host;
    private ProxyRouteItem _selectedProxyRoute;
    private int _portNumber;
    private readonly RoutingMetricCalculator _metricCalculator = new();
    private readonly ProxyMetricsViewModel _metricsViewModel = new();

    public TabItemReverseProxy(IDocProxyRouteRepo repo) {
        _repo = repo;
        CloseCommand = ReactiveCommand.Create(OnClose);
        AddProxyRouteCommand = ReactiveCommand.Create(OnAddProxyRoute);
        MoveUpCommand = ReactiveCommand.Create(OnMoveUp);
        MoveDownCommand = ReactiveCommand.Create(OnMoveDown);
        this._settings = new RestServiceSettings { Enabled = true, PortNumber = 0 };
        this.PortNumber = 9999;
        this._host = "localhost";
        this.ProxyRoutes = new();
    }

    public ObservableCollection<ProxyRouteItem> ProxyRoutes { get; private set; }

    public ServiceCommands ServiceCommands {
        get => _serverCommands;
        set => this.RaiseAndSetIfChanged(ref _serverCommands, value);
    }

    public MainWindowViewModel Main { get; init; }

    public ProxyMetricsViewModel MetricsViewModel {
        get => _metricsViewModel;
    }

    public string Name { get; set; }
    public ICommand CloseCommand { get; }
    public string TabType { get; } = "ReverseProxyTab";
    public string HeaderText { get; } = "Gateway";
    public bool IsServiceHost { get; } = false;
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
            this._settings.PortNumber = value;
        }
    }

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
                if (proxyRules.IsEmpty()) {
                    proxyRules.Add(GetDefaultProxyRoute());
                }

                foreach (var i in proxyRules.OrderBy(t => t.Index)) {
                    var item = new ProxyRouteItem(i, _repo, Remove);
                    this.ProxyRoutes.Add(item);
                }
            });

        //auto-start the reverse proxy
        var startUc = new StartReverseProxyUseCase();
        var proxyHandler = new ProxyRouteHandler(GetProxyRouteFilters, OnMetricReceived);
        await startUc.Start(_settings, proxyHandler).Match(
            CreateServiceCommands,
            exc => { C.Error(exc.ToString()); }
        );
    }

    private void OnMetricReceived(IRoutingMetric metric) {
        _metricCalculator.Run(metric);
        _metricsViewModel.Refresh(_metricCalculator);
    }

    private ProxyRoute[] GetProxyRouteFilters() {
        return this.ProxyRoutes.Select(p => p.ToRoute()).ToArray();
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
        var prox = new ProxyRoute() {
            Index = this.ProxyRoutes.Count,
            IsEnabled = false, //disabled by default. user has to manually enable it
            LastUpdated = DateTime.Now,
            From = new ProxyRoutePart() {
                Host = $"{_host}:{_settings.PortNumber}",
                PathTemplate = "{all}",
                Scheme = "http"
            },
            To = new ProxyRoutePart() {
                Host = "server",
                PathTemplate = "{all}",
                Scheme = "https"
            }
        };

        var item = new ProxyRouteItem(prox, _repo, Remove);
        this.ProxyRoutes.Add(item);
        SaveAll();
    }

    private void SaveAll() {
        ProxyRoutes.Iter((index, p) => {
            p.Index = index;
            p.Save();
        });
    }


    private void OnMoveUp() {
        if (this.SelectedProxyRoute is { }) {
            var thisItem = this.SelectedProxyRoute;
            var pos = this.ProxyRoutes.IndexOf(thisItem);
            if (pos > 0) {
                this.ProxyRoutes.RemoveAt(pos);
                this.ProxyRoutes.Insert(pos - 1, thisItem);
                this.SelectedProxyRoute = thisItem;
                SaveAll();
            }
        }
    }

    private void OnMoveDown() {
        if (this.SelectedProxyRoute is { }) {
            var thisItem = this.SelectedProxyRoute;
            var pos = this.ProxyRoutes.IndexOf(thisItem);
            if (pos < this.ProxyRoutes.Count - 1) {
                this.ProxyRoutes.RemoveAt(pos);
                this.ProxyRoutes.Insert(pos + 1, thisItem);
                this.SelectedProxyRoute = thisItem;
                SaveAll();
            }
        }
    }
}
