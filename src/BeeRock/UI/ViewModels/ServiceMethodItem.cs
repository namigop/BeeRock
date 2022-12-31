using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Utils;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ServiceMethodItem : ViewModelBase {
    private int _callCount;
    private bool _canBeSelected;
    private bool _canShow;
    private bool _httpCallIsOk;

    private bool _isExpanded = true;
    private RestMethodInfo _method;
    private ObservableCollection<ParamInfoItem> _paramInfoItems;
    private HttpStatusCodeItem _selectedHttpResponseType;
    private RuleItem _selectedRule;
    private ParamInfoItem _selectedParamInfoItem;


    //For the xaml designer
    public ServiceMethodItem() {
    }

    public ServiceMethodItem(RestMethodInfo info) {
        Method = info;
        HttpCallIsOk = true;

        SetupRulesSelection();
        SetupHttpStatusCodeSelection();
        SetupSubscriptions();

        InitVariableInfo(info);
        ResetResponseCommand = ReactiveCommand.Create(OnResetResponse);
    }

    public bool IsObsolete {
        get => Method.IsObsolete;
    }
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
        set => this.RaiseAndSetIfChanged(ref _method, value);
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

    public double Opacity {
        get => this.IsObsolete ? 0.25 : 1.0;
    }

    private void SetupSubscriptions() {
        //synchronize with the selected http status code
        var d = this.WhenAnyValue(t => t.SelectedRule.Body)
            .Subscribe(text => {
                if (SelectedHttpResponseType != null) SelectedHttpResponseType.DefaultResponse = text;
            });

        disposable.Add(d);

        var s = this.WhenAnyValue(h => h.SelectedHttpResponseType)
            .WhereNotNull()
            .Subscribe(t => {
                if (t != null) {
                    SelectedRule.Body = t.DefaultResponse;
                    SelectedRule.StatusCode = (int)t.StatusCode;
                }
            });

        disposable.Add(s);
    }

    public void Refresh() {
        foreach (var ruleItem in Rules) ruleItem.Refresh();
    }

    private string GetDefaultResponse(RestMethodInfo info) {
        if (info.ReturnType != typeof(void)) return ObjectBuilder.CreateNewInstanceAsJson(info.ReturnType, 0);
        //flyoutforthe complexparam
        return "//Empty response body";
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

        SelectedRule = Rules.First();
    }
}
