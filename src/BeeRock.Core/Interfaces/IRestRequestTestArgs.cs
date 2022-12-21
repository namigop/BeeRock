namespace BeeRock.Core.Interfaces;

public interface IRestRequestTestArgs {
    int StatusCode { get; }
    bool HttpCallIsOk { get; set; }

    string Body { get; }
    int CallCount { get; set; }

    int DelayMsec { get; }
    List<string> ActiveWhenConditions { get; }
    void UpdateDefaultValues(string varName, string newJson);
}
