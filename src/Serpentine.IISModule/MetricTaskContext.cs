using System.Web;

namespace Serpentine.IISModule
{
    internal interface IMetricTaskContext
    {
        HttpContextBase HttpContext { get; set; }
        IMetricsResponse MetricsResponse { get; }
        IApplicationStorage ApplicationStorage { get; }
    }

    internal class MetricTaskContext : IMetricTaskContext
    {
        public HttpContextBase HttpContext { get; set; }

        public IMetricsResponse MetricsResponse { get; }

        public IApplicationStorage ApplicationStorage { get; }

        public MetricTaskContext(IApplicationStorage storage, IMetricsResponse metricsResponse)
        {
            ApplicationStorage = storage;
            MetricsResponse = metricsResponse;
        }
    }
}