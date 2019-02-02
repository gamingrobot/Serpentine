using System.Net.Mime;
using System.Web;
using Serpentine.IISModule.Tasks.Helpers;

namespace Serpentine.IISModule.Tasks
{
    internal class RequestTimingTask : IMetricsTask
    {
        private readonly HttpApplication _application;
        private readonly RequestTimer _requestTimer;

        public RequestTimingTask(HttpApplication application)
        {
            _application = application;
            _requestTimer = new RequestTimer();
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

            _application.Context.Response.AppendHeader("X-Serpentine-RequestTime", _requestTimer.GetRequestMilliseconds().ToString());
            _application.Context.Response.AppendHeader("X-Serpentine-HandlerTime", _requestTimer.GetHandlerMilliseconds().ToString());


            //Inject html
            if (_application.Context.Response.ContentType == MediaTypeNames.Text.Html)
            {
                _application.Context.Response.Write($"RequestTime: {_requestTimer.GetRequestMilliseconds()}ms<br/>");
                _application.Context.Response.Write($"HandlerTime: {_requestTimer.GetHandlerMilliseconds()}ms<br/>");
            }
        }
    }
}
