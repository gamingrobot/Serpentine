using Serpentine.IISModule.Tasks.Helpers;

namespace Serpentine.IISModule.Tasks
{
    internal class RequestTimingTask : IMetricTask
    {
        private readonly IRequestTimer _requestTimer;
        private readonly IMetricTaskContext _taskContext;

        public RequestTimingTask(IMetricTaskContext taskContext) : this(taskContext, new RequestTimer())
        {
        }

        internal RequestTimingTask(IMetricTaskContext taskContext, IRequestTimer requestTimer)
        {
            _taskContext = taskContext;
            _requestTimer = requestTimer;
        }

        public void BeginRequest()
        {
            _requestTimer.Reset();
            _requestTimer.StartRequestTimer();
        }

        public void PreHandler()
        {
            _requestTimer.StartHandlerTimer();
        }

        public void PostHandler()
        {
            _requestTimer.StopHandlerTimer();
        }

        public void EndRequest()
        {
            _requestTimer.StopRequestTimer();

            _taskContext.MetricsResponse.AddMetric("request-time", "Request Time",
                _requestTimer.GetRequestMilliseconds(), "ms");
            _taskContext.MetricsResponse.AddMetric("request-handler-time", "Request Handler Time",
                _requestTimer.GetHandlerMilliseconds(), "ms");
        }
    }
}