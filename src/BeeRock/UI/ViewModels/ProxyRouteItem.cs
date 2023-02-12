using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.DeleteServiceRuleSets;
using BeeRock.Core.UseCases.SaveRouteRule;
using BeeRock.Core.Utils;
using Community.CsharpSqlite;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ProxyRouteItem : ViewModelBase {
    private readonly IDocProxyRouteRepo _proxyRouteRepo;
    private readonly Action<ProxyRouteItem> _remove;
    private string _fromHost;
    private string _fromPathTemplate;
    private string _fromScheme;
    private bool _isEnabled;
    private string _toHost;
    private string _toPathTemplate;
    private string _toScheme;
    private bool _updateInProgress;

    public ProxyRouteItem(ProxyRoute proxyRoute, IDocProxyRouteRepo proxyRouteRepo, Action<ProxyRouteItem> remove) {
        _proxyRouteRepo = proxyRouteRepo;
        _remove = remove;

        _isEnabled = proxyRoute.IsEnabled;
        _fromHost = proxyRoute.From.Host;
        _fromScheme = proxyRoute.From.Scheme;
        _fromPathTemplate = proxyRoute.From.PathTemplate;
        _toHost = proxyRoute.To.Host;
        _toScheme = proxyRoute.To.Scheme;
        _toPathTemplate = proxyRoute.To.PathTemplate;


        DeleteCommand = ReactiveCommand.CreateFromTask<object, Unit>(OnDelete);
        DocId = proxyRoute.DocId;
        Index = proxyRoute.Index;

        this.WhenAnyValue(
                t => t.FromHost,
                t => t.FromScheme,
                t => t.FromPathTemplate,
                t => t.ToHost,
                t => t.ToScheme,
                t => t.ToPathTemplate)
            .Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(t => Save())
            .Void(d => disposable.Add(d));
    }

    public void Save() {
        if (_updateInProgress)
            return;

        _updateInProgress = true;
        var uc = new SaveProxyRouteUseCase(_proxyRouteRepo);
        _ = uc.Save(this.ToRoute())
            .Match(
                docId => {
                    _updateInProgress = false;
                    this.DocId = docId;
                },
                exc => {
                    _updateInProgress = false;
                    C.Error(exc.ToString());
                });
    }

    private string DocId { get; set; }

    public ICommand DeleteCommand { get; init; }

    public int Index { get; set; }

    public bool IsEnabled {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    public string FromHost {
        get => _fromHost;
        set => this.RaiseAndSetIfChanged(ref _fromHost, value);
    }

    public string FromScheme {
        get => _fromScheme;
        set => this.RaiseAndSetIfChanged(ref _fromScheme, value);
    }

    public string FromPathTemplate {
        get => _fromPathTemplate;
        set => this.RaiseAndSetIfChanged(ref _fromPathTemplate, value?.TrimStart('/'));
    }


    public string ToHost {
        get => _toHost;
        set {
            this.RaiseAndSetIfChanged(ref _toHost, value);
            this.RaisePropertyChanged(nameof(ToFullUrl));
        }
    }

    public string ToScheme {
        get => _toScheme;
        set {
            this.RaiseAndSetIfChanged(ref _toScheme, value);
            this.RaisePropertyChanged(nameof(ToFullUrl));
        }
    }

    public string ToPathTemplate {
        get => _toPathTemplate;
        set {
            this.RaiseAndSetIfChanged(ref _toPathTemplate, value?.TrimStart('/'));
            this.RaisePropertyChanged(nameof(ToFullUrl));
        }
    }

    public string ToFullUrl {
        get => $"{ToScheme}://{ToHost}/{ToPathTemplate}";
    }


    private async Task<Unit> OnDelete(object arg) {
        if (Convert.ToBoolean(arg)) {
            var uc = new DeleteProxyRouteUseCase(_proxyRouteRepo);
            var t = uc.Delete(DocId);
            await t.Match(
                o => { _remove(this); },
                exc => {
                    var msg = "Unable to delete the route filter. Dang it.";
                    var msBoxStandardWindow = MessageBoxManager
                        .GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                            ButtonDefinitions = ButtonEnum.Ok,
                            ContentTitle = "Please Confirm",
                            ContentMessage = msg,
                            Icon = Icon.Question
                        });

                    _ = msBoxStandardWindow.Show();
                }
            );
        }

        return Unit.Default;
    }

    public ProxyRoute ToRoute() {
        return new ProxyRoute() {
            Index = this.Index,
            IsEnabled = this.IsEnabled,
            DocId = this.DocId,
            From = new ProxyRoutePart() {
                Host = this.FromHost,
                PathTemplate = this.FromPathTemplate,
                Scheme = this.FromScheme
            },
            To = new ProxyRoutePart() {
                Host = this.ToHost,
                PathTemplate = this.ToPathTemplate,
                Scheme = this.ToScheme
            }
        };
    }
}
