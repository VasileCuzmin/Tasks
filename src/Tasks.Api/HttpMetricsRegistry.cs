using Prometheus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tasks.Api
{
    public static class HttpMetricsRegistry
    {
        public static void MeasureHttpTrafficMetric(string path, string method, int statusCode)
        {
            var counterTotal = Metrics.CreateCounter("http_requests_total", "HTTP Total Requests", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });

            var counterHttp500Status = Metrics.CreateCounter("http_status_500_total", "HTTP 500 Requests", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });

            if (statusCode == 500)
            {
                counterHttp500Status.Labels(path, method, "500").Inc();
            }
            else
            {
                if (path != "/metrics")
                {
                    counterTotal.Labels(path, method, statusCode.ToString()).Inc();
                }
            }
        }
        public async static Task MeasureHttpLatencyMetric(string path, Func<Task> action)
        {
            Histogram latencyHistogram = Metrics
                           .CreateHistogram("http_requests_latency", "request latency by path",
                               new HistogramConfiguration
                               {
                                   LabelNames = new string[] { "route" },
                                   Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
                               });

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await action.Invoke();

            stopWatch.Stop();
            latencyHistogram
               .WithLabels(path)
               .Observe(stopWatch.Elapsed.TotalSeconds);
        }
    }
}