using BeeRock.Core.Interfaces;
using BeeRock.UI.ViewModels;

namespace BeeRock.UI;

public class RestRequestTestArgs : IRestRequestTestArgs {
    private const string HeaderKey = "header";
    private readonly ServiceMethodItem _methodItem;

    public RestRequestTestArgs(ServiceMethodItem methodItem) {
        _methodItem = methodItem;
        StatusCode = (int)methodItem.SelectedHttpResponseType.StatusCode;
        Body = methodItem.SelectedRule.Body ?? "//comment. will be ignored";
        DelayMsec = methodItem.SelectedRule.DelaySec * 1000;
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

    public int DelayMsec { get; }

    public List<string> ActiveWhenConditions { get; }

    public void UpdateDefaultValues(string varName, string newJson) {
        var paramInfoItem = _methodItem.ParamInfoItems.FirstOrDefault(p => p.Name == varName);
        if (paramInfoItem != null) paramInfoItem.DefaultJson = newJson;
    }
}
