using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;


public class ServiceMethodItem : ViewModelBase {
    private int _callCount;
    private bool _canBeSelected;
    private bool _canShow;
    private bool _httpCallIsActive;
    private bool _isExpanded = true;
    private RestMethodInfo _method;
    private HttpStatusCodeItem _selectedHttpResponseType;
    private RuleItem _selectedRule;

    //For the xaml designer
    public ServiceMethodItem() {
    }

    public ServiceMethodItem(RestMethodInfo info) {
        Method = info;

        SetupRulesSelection();
        SetupHttpStatusCodeSelection();

        //synchronize with the selected http status code
        this.WhenAnyValue(t => t.SelectedRule.Body)
            .Subscribe(text => {
                if (SelectedHttpResponseType != null) SelectedHttpResponseType.DefaultResponse = text;
            });

        this.WhenAnyValue(h => h.SelectedHttpResponseType)
            .WhereNotNull()
            .Subscribe(t => {
                if (t != null) {
                    SelectedRule.Body = t.DefaultResponse;
                    SelectedRule.StatusCode = (int)t.StatusCode;
                }
            });

        InitVariableInfo(info);

        ResetResponseCommand = ReactiveCommand.Create(OnResetResponse);
    }

    public ICommand ResetResponseCommand { get; set; }

    public RuleItem SelectedRule {
        get => _selectedRule;
        set => this.RaiseAndSetIfChanged(ref _selectedRule, value);
    }

    public ObservableCollection<RuleItem> Rules { get; private set; }

    public string VariablesInfo { get; private set; }


    public RestMethodInfo Method {
        get => _method;
        set => this.RaiseAndSetIfChanged(ref _method, value);
    }

    public string Color {
        get {
            if (Method.HttpMethod.ToUpper() == "POST")
                return HttpMethodColor.Post;
            if (Method.HttpMethod.ToUpper() == "PUT")
                return HttpMethodColor.Put;
            if (Method.HttpMethod.ToUpper() == "DELETE")
                return HttpMethodColor.Delete;

            return HttpMethodColor.Get;
        }
    }

    public string Color2 { get; } = "Transparent";

    public bool IsExpanded {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public bool CanShow {
        get => _canShow;
        set => this.RaiseAndSetIfChanged(ref _canShow, value);
    }


    public List<HttpStatusCodeItem> HttpResponseTypes { get; set; }

    public HttpStatusCodeItem SelectedHttpResponseType {
        get => _selectedHttpResponseType;
        set => this.RaiseAndSetIfChanged(ref _selectedHttpResponseType, value);
    }

    public bool HttpCallIsActive {
        get => _httpCallIsActive;
        set {
            if (_httpCallIsActive == false && value) CallCount += 1;

            this.RaiseAndSetIfChanged(ref _httpCallIsActive, value);
        }
    }

    public string CallCountDisplay => $"{CallCount} calls";

    public bool HasCalls => CallCount > 0;

    public int CallCount {
        get => _callCount;
        set {
            this.RaiseAndSetIfChanged(ref _callCount, value);
            this.RaisePropertyChanged(nameof(CallCountDisplay));
            this.RaisePropertyChanged(nameof(HasCalls));
        }
    }

    public bool CanBeSelected {
        get => _canBeSelected;
        set => this.RaiseAndSetIfChanged(ref _canBeSelected, value);
    }

    private void OnResetResponse() {
        var json = GetDefaultResponse(Method);
        SelectedRule.Body = json;
    }

    private void SetupRulesSelection() {
        Rules = new ObservableCollection<RuleItem>(Method.Rules.Select(r => new RuleItem(r)));
        if (!Rules.Any()) {
            var r = new RuleItem { Body = GetDefaultResponse(Method) };
            Rules.Add(r);
            Method.Rules.Add(r.Rule);
        }

        SelectedRule = Rules.First();
    }

    private void InitVariableInfo(RestMethodInfo info) {
        var variables =
            info.Parameters.Select(p => new ParamInfoItem { Name = p.Name, Type = p.Type, TypeName = p.TypeName })
                .Then(i => new ObservableCollection<ParamInfoItem>(i));

        VariablesInfo = string.Join(Environment.NewLine, variables.Select(v => v.Display));
    }


    private string GetDefaultResponse(RestMethodInfo info) {
        if (info.ReturnType != typeof(void)) {
            return ObjectBuilder.CreateNewInstanceAsJson(info.ReturnType, 0);
            //flyoutforthe complexparam
        }

        return "//Empty response body";
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

    public void Refresh() {
        foreach (var ruleItem in Rules) ruleItem.Refresh();
    }
}
