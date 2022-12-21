using BeeRock.Adapters.UI.ViewModels;
using BeeRock.Core.Interfaces;

namespace BeeRock.Adapters.UI;

public class RestRequestTestArgs : IRestRequestTestArgs {
    private const string HeaderKey = "header";
    private readonly ServiceMethodItem _methodItem;

    public RestRequestTestArgs(ServiceMethodItem methodItem) {
        _methodItem = methodItem;
        StatusCode = (int)methodItem.SelectedHttpResponseType.StatusCode;
        Body = methodItem.SelectedRule.Body;
        ActiveWhenConditions = methodItem.SelectedRule.Conditions
            .Where(w => w.IsActive)
            .Select(w => w.BoolExpression)
            .ToList();
    }

    public int StatusCode { get; }

    public bool HttpCallIsOk {
        get => _methodItem.HttpCallIsOk;
        set => _methodItem.HttpCallIsOk = value;
    }

    public string Body { get; }

    public int CallCount {
        get => _methodItem.CallCount;
        set => _methodItem.CallCount = value;
    }

    public List<string> ActiveWhenConditions { get; }

    public void UpdateDefaultValues(string varName, string newJson) {
        var paramInfoItem = _methodItem.ParamInfoItems.First(p => p.Name == varName);
        paramInfoItem.DefaultJson = newJson;
    }
}
