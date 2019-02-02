using System;
using System.Net.Mime;
using System.Web;

namespace Serpentine.IISModule
{
    public class SerpentineModule : IHttpModule
    {
        private const string RequestTimerKey = "SerpentineRequestTimer";

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
            context.EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var context = app.Context;

            var requestTimer = new RequestTimer();
            context.Items.Add(RequestTimerKey, requestTimer);

            requestTimer.StartRequestTimer();
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var context = app.Context;
            var requestTimer = (RequestTimer) context.Items[RequestTimerKey];

            requestTimer.StartHandlerTimer();
        }

        private void OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var context = app.Context;
            var requestTimer = (RequestTimer) context.Items[RequestTimerKey];

            requestTimer.StopHandlerTimer();
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var context = app.Context;

            var requestTimer = (RequestTimer) context.Items[RequestTimerKey];
            requestTimer.StopRequestTimer();

            //TODO get body size, calculate min, max, average

            context.Response.AppendHeader("X-Serpentine-RequestTime", requestTimer.GetRequestMilliseconds().ToString());
            context.Response.AppendHeader("X-Serpentine-HandlerTime", requestTimer.GetHandlerMilliseconds().ToString());

            //Inject html
            if (context.Response.ContentType == MediaTypeNames.Text.Html)
            {
                context.Response.Write($"RequestTime: {requestTimer.GetRequestMilliseconds()}ms<br/>HandlerTime: {requestTimer.GetHandlerMilliseconds()}ms");
            }
        }

        public void Dispose()
        {
        }
    }
}
