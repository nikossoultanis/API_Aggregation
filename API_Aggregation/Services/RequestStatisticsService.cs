using System.Collections.Concurrent;

namespace API_Aggregation.Services
{
    public class RequestStatisticsService
    {
        private readonly ConcurrentDictionary<string, List<long>> _requestTimes = new ConcurrentDictionary<string, List<long>>();

        // storing every info for every api call into a dict
        public void RecordRequest(string apiName, long responseTime)
        {
            if (!_requestTimes.ContainsKey(apiName))
            {
                _requestTimes[apiName] = new List<long>();
            }
            _requestTimes[apiName].Add(responseTime);
        }

        public Dictionary<string, ApiStatistics> GetStatistics()
        {
            var statistics = new Dictionary<string, ApiStatistics>();

            // for each info saved, we create a performance buckets.
            // and print it out to the user.
            foreach (var api in _requestTimes)
            {
                var totalRequests = api.Value.Count;
                var averageResponseTime = api.Value.Average();
                var performanceBuckets = new PerformanceBuckets
                {
                    Fast = api.Value.Count(time => time < 100),
                    Average = api.Value.Count(time => time >= 100 && time <= 200),
                    Slow = api.Value.Count(time => time > 200)
                };

                statistics[api.Key] = new ApiStatistics
                {
                    TotalRequests = totalRequests,
                    AverageResponseTime = averageResponseTime,
                    PerformanceBuckets = performanceBuckets
                };
            }

            return statistics;
        }
    }

    public class ApiStatistics
    {
        public int TotalRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public PerformanceBuckets PerformanceBuckets { get; set; }
    }

    public class PerformanceBuckets
    {
        public int Fast { get; set; }
        public int Average { get; set; }
        public int Slow { get; set; }
    }
}
