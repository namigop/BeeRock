namespace BeeRock.Core.Interfaces;

public interface IRestRequestTestArg {
    List<string> ActiveWhenConditions { get; }
    string Body { get; }
    int DelayMsec { get; }
    int StatusCode { get; }

}
public interface IRestRequestTestArgs {

    List<IRestRequestTestArg> Args { get; }
    int CallCount { get; set; }

    bool HttpCallIsOk { get; set; }

    void UpdateDefaultValues(string varName, string newJson);
}
