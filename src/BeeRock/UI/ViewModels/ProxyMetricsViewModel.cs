using BeeRock.Core.Entities;
using ReactiveUI;

namespace BeeRock.UI.ViewModels;

public class ProxyMetricsViewModel : ViewModelBase {
    private int _errorCount;
    private double _httpDeleteAveRespTime;
    private int _httpDeleteCount;
    private double _httpGetAveRespTime;
    private int _httpGetCount;
    private double _httpOptionsAveRespTime;
    private int _httpOptionsCount;
    private double _httpPatchAveRespTime;
    private int _httpPatchCount;
    private double _httpPostAveRespTime;
    private int _httpPostCount;
    private double _httpPutAveRespTime;
    private int _httpPutCount;
    private int _okCount;
    private double _httpDeleteAveRespSize;
    private double _httpPostAveRespSize;
    private double _httpPutAveRespSize;
    private double _httpGetAveRespSize;

    public double HttpPatchAveRespTime {
        get => _httpPatchAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpPatchAveRespTime, value);
    }

    public double HttpOptionsAveRespTime {
        get => _httpOptionsAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpOptionsAveRespTime, value);
    }

    public double HttpDeleteAveRespTime {
        get => _httpDeleteAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpDeleteAveRespTime, value);
    }

    public double HttpPostAveRespTime {
        get => _httpPostAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpPostAveRespTime, value);
    }

    public double HttpPutAveRespTime {
        get => _httpPutAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpPutAveRespTime, value);
    }

    public double HttpGetAveRespTime {
        get => _httpGetAveRespTime;
        set => this.RaiseAndSetIfChanged(ref _httpGetAveRespTime, value);
    }

    public int HttpPatchCount {
        get => _httpPatchCount;
        set => this.RaiseAndSetIfChanged(ref _httpPatchCount, value);
    }

    public int HttpOptionsCount {
        get => _httpOptionsCount;
        set => this.RaiseAndSetIfChanged(ref _httpOptionsCount, value);
    }

    public int HttpDeleteCount {
        get => _httpDeleteCount;
        set => this.RaiseAndSetIfChanged(ref _httpDeleteCount, value);
    }

    public int HttpPostCount {
        get => _httpPostCount;
        set => this.RaiseAndSetIfChanged(ref _httpPostCount, value);
    }

    public int HttpPutCount {
        get => _httpPutCount;
        set => this.RaiseAndSetIfChanged(ref _httpPutCount, value);
    }

    public int HttpGetCount {
        get => _httpGetCount;
        set => this.RaiseAndSetIfChanged(ref _httpGetCount, value);
    }

    public int OKCount {
        get => _okCount;
        set => this.RaiseAndSetIfChanged(ref _okCount, value);
    }

    public int ErrorCount {
        get => _errorCount;
        set => this.RaiseAndSetIfChanged(ref _errorCount, value);
    }

    public void Refresh(RoutingMetricCalculator calc) {
        ErrorCount = calc.ErrorCount;
        OKCount = calc.OkCount;
        HttpGetCount = calc.CallCountPerMethod.ContainsKey("GET") ? calc.CallCountPerMethod["GET"] : 0;
        HttpPutCount = calc.CallCountPerMethod.ContainsKey("PUT") ? calc.CallCountPerMethod["PUT"] : 0;
        HttpPostCount = calc.CallCountPerMethod.ContainsKey("POST") ? calc.CallCountPerMethod["POST"] : 0;
        HttpDeleteCount = calc.CallCountPerMethod.ContainsKey("DELETE") ? calc.CallCountPerMethod["DELETE"] : 0;
        HttpOptionsCount = calc.CallCountPerMethod.ContainsKey("OPTIONS") ? calc.CallCountPerMethod["OPTIONS"] : 0;
        HttpPatchCount = calc.CallCountPerMethod.ContainsKey("PATCH") ? calc.CallCountPerMethod["PATCH"] : 0;

        HttpGetAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("GET") ? calc.AverageRespTimePerMethod["GET"] : 0;
        HttpPutAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("PUT") ? calc.AverageRespTimePerMethod["PUT"] : 0;
        HttpPostAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("POST") ? calc.AverageRespTimePerMethod["POST"] : 0;
        HttpDeleteAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("DELETE") ? calc.AverageRespTimePerMethod["DELETE"] : 0;
        HttpOptionsAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("OPTIONS") ? calc.AverageRespTimePerMethod["OPTIONS"] : 0;
        HttpPatchAveRespTime = calc.AverageRespTimePerMethod.ContainsKey("PATCH") ? calc.AverageRespTimePerMethod["PATCH"] : 0;

        //Size in KB
        HttpGetAveRespSize = calc.AverageRespSizePerMethod.ContainsKey("GET") ? calc.AverageRespSizePerMethod["GET"] / 1024.0 : 0;
        HttpPutAveRespSize = calc.AverageRespSizePerMethod.ContainsKey("PUT") ? calc.AverageRespSizePerMethod["PUT"] / 1024.0 : 0;
        HttpPostAveRespSize = calc.AverageRespSizePerMethod.ContainsKey("POST") ? calc.AverageRespSizePerMethod["POST"] / 1024.0 : 0;
        HttpDeleteAveRespSize = calc.AverageRespSizePerMethod.ContainsKey("DELETE") ? calc.AverageRespSizePerMethod["DELETE"] / 1024.0 : 0;
    }

    public double HttpDeleteAveRespSize {
        get => _httpDeleteAveRespSize;
        set => this.RaiseAndSetIfChanged(ref _httpDeleteAveRespSize, value);
    }

    public double HttpPostAveRespSize {
        get => _httpPostAveRespSize;
        set => this.RaiseAndSetIfChanged(ref _httpPostAveRespSize, value);
    }

    public double HttpPutAveRespSize {
        get => _httpPutAveRespSize;
        set => this.RaiseAndSetIfChanged(ref _httpPutAveRespSize, value);
    }

    public double HttpGetAveRespSize {
        get => _httpGetAveRespSize;
        set => this.RaiseAndSetIfChanged(ref _httpGetAveRespSize, value);
    }
}
