using System.Collections.ObjectModel;
using System.Net;
using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using BeeRock.Core.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceMethodItem : ViewModelBase {
    private bool _canShow = true;
    private RestMethodInfo _method;
    private string _responseText;
    private HttpStatusCodeItem _selectedHttpResponseType;

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
            new WhenConditionItem { IsActive = true, BoolExpression = "True" } //pass-through
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
