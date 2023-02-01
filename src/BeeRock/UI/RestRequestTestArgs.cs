using BeeRock.Core.Interfaces;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI;

public class RestRequestTestArgs : IRestRequestTestArgs {
    private readonly ServiceMethodItem _methodItem;

    public RestRequestTestArgs(ServiceMethodItem methodItem, string swaggerUrl) {
        SwaggerUrl = swaggerUrl;
        _methodItem = methodItem;
        HttpMethod = methodItem.Method.HttpMethod;
        RouteTemplate = methodItem.Method.RouteTemplate;
        Args = methodItem.Rules.Select(t => new Arg {
            StatusCode = t.StatusCode,
            Body = t.Body,
            Name = t.Name,
            DelayMsec = t.DelaySec,
            ActiveWhenConditions = t.Conditions.Where(w => w.IsActive).Select(w => w.BoolExpression ?? "False").ToList()
        }).Cast<IRestRequestTestArg>().ToList();
    }

    public string RouteTemplate { get; init; }
    public string HttpMethod { get; init; }

    public List<IRestRequestTestArg> Args { get; init; }

    public string SwaggerUrl { get; }

    public bool HttpCallIsOk {
        get => _methodItem.HttpCallIsOk;
        set => _methodItem.HttpCallIsOk = value;
    }

    public string Error {
        get => _methodItem.Error;
        set => _methodItem.Error = value;
    }

    //public string Body { get; }

    public int CallCount {
        get => _methodItem.CallCount;
        set => _methodItem.CallCount = value;
    }

    public void UpdateDefaultValues(string varName, string newJson) {
        var paramInfoItem = _methodItem.ParamInfoItems.FirstOrDefault(p => p.Name == varName);
        if (paramInfoItem != null) paramInfoItem.DefaultJson = newJson;
    }

    public class Arg : IRestRequestTestArg {
        public int StatusCode { get; init; }
        public string Body { get; init; }
        public int DelayMsec { get; init; }
        public string Name { get; init; }
        public List<string> ActiveWhenConditions { get; init; }
    }
}
