using BeeRock.Core.Interfaces;

namespace BeeRock.Core.Entities;

public class RoutingMetricCalculator {
    public Dictionary<string, int> CallCountPerMethod { get; } = new();
    public Dictionary<string, double> AverageRespTimePerMethod { get; } = new();
    public Dictionary<string, double> AverageRespSizePerMethod { get; } = new();
    public int ErrorCount { get; private set; }
    public int OkCount { get; private set; }

    private static readonly object _sync = new();

    public void Run(IRoutingMetric metric) {
        lock (_sync) {
            if (!AverageRespTimePerMethod.ContainsKey(metric.HttpMethod)) {
                AverageRespTimePerMethod.Add(metric.HttpMethod, metric.Elapsed.TotalMilliseconds);
                AverageRespSizePerMethod.Add(metric.HttpMethod, metric.ResponseLength.GetValueOrDefault());
                CallCountPerMethod.Add(metric.HttpMethod, 1);
            }
            else {
                var prevTotalRespTime = AverageRespTimePerMethod[metric.HttpMethod] * CallCountPerMethod[metric.HttpMethod];
                var prevTotalRespSize = AverageRespSizePerMethod[metric.HttpMethod] * CallCountPerMethod[metric.HttpMethod];

                CallCountPerMethod[metric.HttpMethod] += 1;
                AverageRespTimePerMethod[metric.HttpMethod] = (prevTotalRespTime + metric.Elapsed.TotalMilliseconds) / CallCountPerMethod[metric.HttpMethod];
                AverageRespSizePerMethod[metric.HttpMethod] = (prevTotalRespSize + metric.ResponseLength.GetValueOrDefault()) / CallCountPerMethod[metric.HttpMethod];
            }

            if (metric.StatusCode >= 400) {
                this.ErrorCount += 1;
            }

            if (metric.StatusCode is >= 200 and < 400) {
                this.OkCount += 1;
            }
        }
    }
}