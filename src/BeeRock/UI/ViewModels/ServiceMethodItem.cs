using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Interfaces;
using BeeRock.Core.UseCases.LoadServiceRuleSets;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem : ViewModelBase {
    private const string empty = "//Empty response body";
    private readonly IDocRuleRepo _ruleRepo;
    private int _callCount;
    private bool _canBeSelected = true;
    private bool _canShow;
    private string _error;
    private bool _httpCallIsOk;
    private string _httpMethodName;

    private bool _isExpanded = true;
    private RestMethodInfo _method;
    private ObservableCollection<ParamInfoItem> _paramInfoItems;
    private string _routeTemplate;
    private HttpStatusCodeItem _selectedHttpResponseType;
    private ParamInfoItem _selectedParamInfoItem;
    private RuleItem _selectedRule;

    //For the xaml designer
    public ServiceMethodItem() {
    }

    public ServiceMethodItem(RestMethodInfo info, IDocRuleRepo ruleRepo) {
        _ruleRepo = ruleRepo;
        Method = info;
        _httpMethodName = info.HttpMethod;
        RouteTemplate = info.RouteTemplate;
        HttpCallIsOk = true;

        SetupRulesSelection();
        SetupHttpStatusCodeSelection();
        SetupSubscriptions();

        InitVariableInfo(info);
        ResetResponseCommand = ReactiveCommand.Create(OnResetResponse);
        PrettifyResponseCommand = ReactiveCommand.Create(OnPrettifyResponse);
        CreateNewRuleCommand = ReactiveCommand.Create(OnCreateNewRule);
        DeleteRuleCommand = ReactiveCommand.Create<RuleItem>(OnDeleteRule);
    }

    public string HttpMethodName {
        get => _httpMethodName;
        set {
            if (IsServiceDynamic) {
                this.RaiseAndSetIfChanged(ref _httpMethodName, value);
                this.RaisePropertyChanged(nameof(Color));
            }
        }
    }

    public string RouteTemplate {
        get => _routeTemplate;
        set => this.RaiseAndSetIfChanged(ref _routeTemplate, value);
    }


    public bool IsObsolete => Method.IsObsolete;

    public int CallCount {
        get => _callCount;
        set {
            _ = this.RaiseAndSetIfChanged(ref _callCount, value);

            this.RaisePropertyChanged(nameof(CallCountDisplay));
            this.RaisePropertyChanged(nameof(HasCalls));
        }
    }

    public string CallCountDisplay => _callCount > 0 ? $"{_callCount} calls" : "";


    public bool CanBeSelected {
        get => _canBeSelected;
        set => this.RaiseAndSetIfChanged(ref _canBeSelected, value);
    }

    public bool CanShow {
        get => _canShow;
        set => this.RaiseAndSetIfChanged(ref _canShow, value);
    }

    public string Color {
        get {
            if (HttpMethodName == "POST")
                return HttpMethodColor.Post;
            if (HttpMethodName == "PUT")
                return HttpMethodColor.Put;
            if (HttpMethodName == "DELETE")
                return HttpMethodColor.Delete;
            if (HttpMethodName == "PATCH")
                return HttpMethodColor.Patch;

            return HttpMethodColor.Get;
        }
    }


    public bool HasCalls => CallCount > 0;

    public bool HttpCallIsOk {
        get => _httpCallIsOk;
        set => this.RaiseAndSetIfChanged(ref _httpCallIsOk, value);
    }

    public List<HttpStatusCodeItem> HttpResponseTypes { get; set; }

    public bool IsExpanded {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public RestMethodInfo Method {
        get => _method;
        private set => this.RaiseAndSetIfChanged(ref _method, value);
    }

    public ParamInfoItem SelectedParamInfoItem {
        get => _selectedParamInfoItem;
        set => this.RaiseAndSetIfChanged(ref _selectedParamInfoItem, value);
    }

    public ObservableCollection<ParamInfoItem> ParamInfoItems {
        get => _paramInfoItems;
        set => this.RaiseAndSetIfChanged(ref _paramInfoItems, value);
    }

    public ICommand ResetResponseCommand { get; set; }

    public ObservableCollection<RuleItem> Rules { get; private set; }

    public HttpStatusCodeItem SelectedHttpResponseType {
        get => _selectedHttpResponseType;
        set => this.RaiseAndSetIfChanged(ref _selectedHttpResponseType, value);
    }

    public RuleItem SelectedRule {
        get => _selectedRule;
        set => this.RaiseAndSetIfChanged(ref _selectedRule, value);
    }

    public double Opacity => IsObsolete ? 0.45 : 1.0;

    public string Error {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }

    public ICommand PrettifyResponseCommand { get; }
    public bool IsServiceDynamic { get; init; }

    public List<string> HttpMethodNames { get; } = new() {
        HttpMethod.Get.Method,
        HttpMethod.Post.Method,
        HttpMethod.Put.Method,
        HttpMethod.Delete.Method,
        HttpMethod.Patch.Method
    };

    private void SetupSubscriptions() {
        //synchronize with the selected http status code
        this.WhenAnyValue(t => t.SelectedRule.Body)
            .Subscribe(text => {
                if (SelectedHttpResponseType != null)
                    SelectedHttpResponseType.DefaultResponse = text;
            }).Void(d => disposable.Add(d));

        this.WhenAnyValue(h => h.SelectedHttpResponseType)
            .WhereNotNull()
            .Subscribe(t => {
                if (t != null) {
                    SelectedRule.Body = t.DefaultResponse;
                    SelectedRule.StatusCode = (int)t.StatusCode;
                }
            }).Void(d => disposable.Add(d));

        this.WhenAnyValue(t => t.SelectedRule)
            .WhereNotNull()
            .Subscribe(C => {
                var selectedStatusCode = SelectedRule.StatusCode;
                var h = HttpResponseTypes.First(h => (int)h.StatusCode == selectedStatusCode);
                h.DefaultResponse = SelectedRule.Body;
                SelectedHttpResponseType = h;

                foreach (var r in Rules) r.IsSelected = r == SelectedRule;
            })
            .Void(d => disposable.Add(d));
    }

    public void Refresh() {
        if (IsServiceDynamic) {
            Method.HttpMethod = HttpMethodName;
            Method.RouteTemplate = RouteTemplate;
        }

        foreach (var ruleItem in Rules)
            ruleItem.Refresh();
    }

    public static string GetDefaultResponse(RestMethodInfo info) {
        if (info.ReturnType != typeof(void)) {
            if (info.ReturnType == typeof(FileContentResult)) return @"<<bee.FileResp.ToAny(""/path/to/myfile"", ""text/plain"")>>";

            return ObjectBuilder.CreateNewInstanceAsJson(info.ReturnType, 0);
        }

        return empty;
    }

    private void InitVariableInfo(RestMethodInfo info) {
        ParamInfoItems =
            info.Parameters.Select(p => new ParamInfoItem {
                    Name = p.Name,
                    Type = p.Type,
                    TypeName = p.TypeName,
                    DefaultJson = p.DisplayValue ?? ObjectBuilder.CreateNewInstanceAsJson(p.Type, 0)
                })
                .Then(i => new ObservableCollection<ParamInfoItem>(i));

        SelectedParamInfoItem = ParamInfoItems.FirstOrDefault();
    }

    private void OnResetResponse() {
        var json = GetDefaultResponse(Method);
        SelectedRule.Body = json;
    }

    private void OnPrettifyResponse() {
        var resp = _selectedRule.Body.TrimStart();
        if (resp.StartsWith('{') || resp.StartsWith('['))
            try {
                dynamic parsedJson = JsonConvert.DeserializeObject(resp);
                _selectedRule.Body = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (Exception exc) {
                C.Warn($"Unable to prettify json. {exc}");
            }
    }

    private void SetupHttpStatusCodeSelection() {
        HttpResponseTypes = new List<HttpStatusCodeItem>();
        Enum.GetValues<HttpStatusCode>()
            .Where(c => ((int)c >= 200 && (int)c < 300) || (int)c > 400) //only HTTP 2xx, 4xx and 5xx
            .Select(h => new HttpStatusCodeItem { StatusCode = h })
            .Void(h => HttpResponseTypes.AddRange(h));

        var selectedStatusCode = SelectedRule.StatusCode;
        SelectedHttpResponseType = HttpResponseTypes.First(h => (int)h.StatusCode == selectedStatusCode);
    }

    private void SetupRulesSelection() {
        Rules = new ObservableCollection<RuleItem>(Method.Rules.Select(r => new RuleItem(r)));
        if (!Rules.Any()) {
            var r = new RuleItem { Body = GetDefaultResponse(Method) };
            Rules.Add(r);
            Method.Rules.Add(r.Rule);
        }

        SelectedRule = Rules.FirstOrDefault(r => r.IsSelected) ?? Rules.First();
    }

    public async Task Load() {
        var uc = new LoadRuleSetUseCase(_ruleRepo);

        foreach (var ruleItem in Rules.Where(t => !string.IsNullOrWhiteSpace(t.DocId))) {
            if (ruleItem.Body != null)
                //skip if already loaded
                continue;

            var temp = await uc.LoadById(ruleItem.DocId).Match(Result.Create, Result.Error<Rule>);
            if (temp.IsFailed)
                C.Error(temp.Error.ToString());
            else
                ruleItem.From(temp.Value);

            if (ruleItem == SelectedRule) {
                var h = HttpResponseTypes.First(h => (int)h.StatusCode == ruleItem.StatusCode);
                h.DefaultResponse = ruleItem.Body;
                SelectedHttpResponseType = h;
            }

            if (ruleItem.Body == null)
                //something is wrong. We use default values instead
                ruleItem.Body = GetDefaultResponse(Method);
        }
    }
}
