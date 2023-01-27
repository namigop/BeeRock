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
    string SwaggerUrl { get; }

    bool HttpCallIsOk { get; set; }
    string Error { get; set; }

    void UpdateDefaultValues(string varName, string newJson);
}
