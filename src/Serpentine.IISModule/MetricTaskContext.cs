using System.Web;

namespace Serpentine.IISModule
{
    internal interface IMetricTaskContext
    {
        HttpContextBase HttpContext { get; set; }
    }

    internal class MetricTaskContext : IMetricTaskContext
    {
        public HttpContextBase HttpContext { get; set; }
    }
}