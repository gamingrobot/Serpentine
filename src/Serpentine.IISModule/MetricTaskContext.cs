using System.Web;

namespace Serpentine.IISModule
{
    internal interface IMetricTaskContext
    {
        HttpContextBase HttpContext { get; set; }
        IMetricsResponse MetricsResponse { get; }
    }

    internal class MetricTaskContext : IMetricTaskContext
    {
        public HttpContextBase HttpContext { get; set; }

        public IMetricsResponse MetricsResponse { get; }

        public MetricTaskContext() : this(new MetricsResponse())
        {
        }

        internal MetricTaskContext(IMetricsResponse metricsResponse)
        {
            MetricsResponse = metricsResponse;
        }
    }
}