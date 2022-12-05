using BeeRock.Core.Entities;
using BeeRock.Core.Entities.ObjectBuilder;
using Newtonsoft.Json;
using ReactiveUI;

namespace BeeRock.Adapters.UI.ViewModels;

public class ServiceMethodItem : ViewModelBase {
    private bool _canShow = true;
    private RestMethodInfo _method;
    private string _responseText;


    public ServiceMethodItem(RestMethodInfo info) {
        Method = info;
        if (info.ReturnType != typeof(void)) {
            var instance = ObjectBuilder.CreateNewInstance(info.ReturnType, 0);
            try {
                var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                ResponseText = json;
            }
            catch {
            }
        }
    }

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
}