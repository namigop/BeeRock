namespace BeeRock.Core.Interfaces;

public interface IRestRequestTestArgs {
    List<string> ActiveWhenConditions { get; }
    string Body { get; }
    int CallCount { get; set; }
    int DelayMsec { get; }
    bool HttpCallIsOk { get; set; }
    int StatusCode { get; }

    void UpdateDefaultValues(string varName, string newJson);
}