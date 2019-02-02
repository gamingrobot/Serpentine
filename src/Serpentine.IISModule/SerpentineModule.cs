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

            context.Response.Filter = new ResponseSizeFilter(context.Response.Filter);

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

            var sizeFilter = context.Response.Filter;
            ResponseSizeStorage.Instance.UpdateSize(sizeFilter.Length);

            context.Response.AppendHeader("X-Serpentine-RequestTime", requestTimer.GetRequestMilliseconds().ToString());
            context.Response.AppendHeader("X-Serpentine-HandlerTime", requestTimer.GetHandlerMilliseconds().ToString());
            context.Response.AppendHeader("X-Serpentine-RequestSize", sizeFilter.Length.ToString());
            context.Response.AppendHeader("X-Serpentine-RequestSizeMin", ResponseSizeStorage.Instance.MinimumSize.ToString());
            context.Response.AppendHeader("X-Serpentine-RequestSizeMax", ResponseSizeStorage.Instance.MaximumSize.ToString());
            context.Response.AppendHeader("X-Serpentine-RequestSizeAvg", ResponseSizeStorage.Instance.AverageSize.ToString());


            //Inject html
            if (context.Response.ContentType == MediaTypeNames.Text.Html)
            {
                context.Response.Write($"RequestTime: {requestTimer.GetRequestMilliseconds()}ms<br/>");
                context.Response.Write($"HandlerTime: {requestTimer.GetHandlerMilliseconds()}ms<br/>");
                context.Response.Write($"RequestSize: {sizeFilter.Length} bytes<br/>");
                context.Response.Write($"RequestSizeMin: {ResponseSizeStorage.Instance.MinimumSize} bytes<br/>");
                context.Response.Write($"RequestSizeMax: {ResponseSizeStorage.Instance.MaximumSize} bytes<br/>");
                context.Response.Write($"RequestSizeAvg: {ResponseSizeStorage.Instance.AverageSize} bytes<br/>");
            }
        }

        public void Dispose()
        {
        }
    }
}
