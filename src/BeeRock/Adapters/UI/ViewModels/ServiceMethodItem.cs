using System.Collections.ObjectModel;
using System.Net;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceMethodItem : ViewModelBase {
    private int _callCount;
    private bool _canShow;
    private bool _httpCallIsActive;
    private RestMethodInfo _method;
    private string _responseText;
    private HttpStatusCodeItem _selectedHttpResponseType;
    private bool _canBeSelected;
    private bool _isExpanded = true;

    public ServiceMethodItem(RestMethodInfo info) {
        Method = info;

        SetupHttpStatusCodeSelection();
        SetupRequestFilterConditions();

        //synchronize with the selected http status code
        this.WhenAnyValue(t => t.ResponseText)
            .Subscribe(text => {
                if (SelectedHttpResponseType != null)
                    SelectedHttpResponseType.DefaultResponse = text;
            });

        this.WhenAnyValue(h => h.SelectedHttpResponseType)
            .WhereNotNull()
            .Subscribe(t => ResponseText = t?.DefaultResponse);


        InitDefaultResponses(info);
        InitVariableInfo(info);
    }

    public string VariablesInfo { get; private set; }

    public string ResponseText {
        get => _responseText;
        set => this.RaiseAndSetIfChanged(ref _responseText, value);
    }

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
        set => this.RaiseAndSetIfChanged(ref _isExpanded , value);
    }

    public bool CanShow {
        get => _canShow;
        set => this.RaiseAndSetIfChanged(ref _canShow, value);
    }

    public ObservableCollection<WhenConditionItem> WhenConditions { get; set; }
    public List<HttpStatusCodeItem> HttpResponseTypes { get; set; }

    public HttpStatusCodeItem SelectedHttpResponseType {
        get => _selectedHttpResponseType;
        set => this.RaiseAndSetIfChanged(ref _selectedHttpResponseType, value);
    }

    public bool HttpCallIsActive {
        get => _httpCallIsActive;
        set {
            if (_httpCallIsActive == false && value) {
                CallCount += 1;
            }

            this.RaiseAndSetIfChanged(ref _httpCallIsActive, value);
        }
    }

    public string CallCountDisplay {
        get => $"{CallCount} calls";
    }

    public bool HasCalls {
        get => this.CallCount > 0;
    }
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

    private void InitVariableInfo(RestMethodInfo info) {
        var variables =
            info.Parameters.Select(p => new ParamInfoItem { Name = p.Name, Type = p.Type })
                .Then(i => new ObservableCollection<ParamInfoItem>(i));

        VariablesInfo = string.Join(Environment.NewLine, variables.Select(v => v.Display));
    }


    private void InitDefaultResponses(RestMethodInfo info) {
        if (info.ReturnType != typeof(void)) {
            var instance = ObjectBuilder.CreateNewInstance(info.ReturnType, 0);
            try {
                var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                ResponseText = json;
            }
            catch {
                // ignored. User can manually edit the response
            }
        }
        else {
            ResponseText = "//Empty response body";
        }
    }

    private void SetupRequestFilterConditions() {
        WhenConditions = new ObservableCollection<WhenConditionItem> {
            new() { IsActive = true, BoolExpression = "True" } //pass-through
        };
    }

    private void SetupHttpStatusCodeSelection() {
        HttpResponseTypes = new List<HttpStatusCodeItem>();
        Enum.GetValues<HttpStatusCode>()
            .Where(c => ((int)c >= 200 && (int)c < 300) || (int)c > 400) //only HTTP 2xx, 4xx and 5xx
            .Select(h => new HttpStatusCodeItem { StatusCode = h })
            .Void(h => HttpResponseTypes.AddRange(h));
        SelectedHttpResponseType = HttpResponseTypes.First(h => h.StatusCode == HttpStatusCode.OK);
    }
}
