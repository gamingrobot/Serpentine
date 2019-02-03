using System.Net.Mime;
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

            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-RequestTime", _requestTimer.GetRequestMilliseconds().ToString());
            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-HandlerTime", _requestTimer.GetHandlerMilliseconds().ToString());


            //Inject html
            if (_taskContext.HttpContext.Response.ContentType == MediaTypeNames.Text.Html)
            {
                _taskContext.HttpContext.Response.Write($"RequestTime: {_requestTimer.GetRequestMilliseconds()}ms<br/>");
                _taskContext.HttpContext.Response.Write($"HandlerTime: {_requestTimer.GetHandlerMilliseconds()}ms<br/>");
            }
        }
    }
}