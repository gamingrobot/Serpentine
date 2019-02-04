using System.Web;

namespace Serpentine.IISModule
{
    internal interface IMetricTaskContext
    {
        /// <summary>
        /// HttpContext for the request
        /// </summary>
        HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Used for adding metrics to be rendered out in the response
        /// </summary>
        IMetricsResponse MetricsResponse { get; }

        /// <summary>
        /// Used for storage of data for the worker process
        /// </summary>
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