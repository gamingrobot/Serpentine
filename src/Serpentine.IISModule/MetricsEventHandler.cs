using System.Collections.Generic;
using System.Web;
using Serpentine.IISModule.Tasks;

namespace Serpentine.IISModule
{
    internal interface IMetricsEventHandler
    {
        void BeginRequest(HttpContextBase context);
        void PreHandler(HttpContextBase context);
        void PostHandler(HttpContextBase context);
        void EndRequest(HttpContextBase context);
    }

    internal class MetricsEventHandler : IMetricsEventHandler
    {
        private readonly IList<IMetricTask> _metricTasks;
        private readonly IMetricTaskContext _taskContext;

        public MetricsEventHandler(IMetricTaskContext taskContext)
        {
            _taskContext = taskContext;
            _metricTasks = new List<IMetricTask>
            {
                new RequestTimingTask(_taskContext),
                new ResponseSizeTask(_taskContext)
            };
        }

        public void BeginRequest(HttpContextBase context)
        {
            _taskContext.HttpContext = context;
            foreach (var metricsTask in _metricTasks)
            {
                metricsTask.BeginRequest();
            }
        }

        public void PreHandler(HttpContextBase context)
        {
            _taskContext.HttpContext = context;
            foreach (var metricsTask in _metricTasks)
            {
                metricsTask.PreHandler();
            }
        }

        public void PostHandler(HttpContextBase context)
        {
            _taskContext.HttpContext = context;
            foreach (var metricsTask in _metricTasks)
            {
                metricsTask.PostHandler();
            }
        }

        public void EndRequest(HttpContextBase context)
        {
            _taskContext.HttpContext = context;
            foreach (var metricsTask in _metricTasks)
            {
                metricsTask.EndRequest();
            }

            _taskContext.MetricsResponse.Render(context.Response);
        }
    }
}