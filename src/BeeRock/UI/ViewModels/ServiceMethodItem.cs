using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Interfaces;
using BeeRock.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public partial class ServiceMethodItem : ViewModelBase {
    private const string empty = "//Empty response body";
    private readonly IDocRuleRepo _ruleRepo;
    private int _callCount;
    private bool _canBeSelected;
    private bool _canShow;
    private string _error;
    private bool _httpCallIsOk;

    private bool _isExpanded = true;
    private RestMethodInfo _method;
    private ObservableCollection<ParamInfoItem> _paramInfoItems;
    private HttpStatusCodeItem _selectedHttpResponseType;
    private ParamInfoItem _selectedParamInfoItem;
    private RuleItem _selectedRule;

    //For the xaml designer
    public ServiceMethodItem() {
    }

    public ServiceMethodItem(RestMethodInfo info, IDocRuleRepo ruleRepo) {
        _ruleRepo = ruleRepo;
        Method = info;
        HttpCallIsOk = true;

        SetupRulesSelection();
        SetupHttpStatusCodeSelection();
        SetupSubscriptions();

        InitVariableInfo(info);
        ResetResponseCommand = ReactiveCommand.Create(OnResetResponse);
        CreateNewRuleCommand = ReactiveCommand.Create(OnCreateNewRule);
        DeleteRuleCommand = ReactiveCommand.Create<RuleItem>(OnDeleteRule);
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
            if (Method.HttpMethod.ToUpper() == "POST")
                return HttpMethodColor.Post;
            if (Method.HttpMethod.ToUpper() == "PUT")
                return HttpMethodColor.Put;
            if (Method.HttpMethod.ToUpper() == "DELETE")
                return HttpMethodColor.Delete;
            if (Method.HttpMethod.ToUpper() == "PATCH")
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

    private void SetupSubscriptions() {
        //synchronize with the selected http status code
        this.WhenAnyValue(t => t.SelectedRule.Body)
            .Subscribe(text => {
                if (SelectedHttpResponseType != null) SelectedHttpResponseType.DefaultResponse = text;
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
        foreach (var ruleItem in Rules) ruleItem.Refresh();
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
}
