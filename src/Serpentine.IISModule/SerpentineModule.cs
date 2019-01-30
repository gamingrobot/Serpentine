using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Web;

namespace Serpentine.IISModule
{
    public class SerpentineModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
            context.EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;

            var stopwatch = Stopwatch.StartNew();
            context.Items.Add("SerpentineRequestTimer", stopwatch);
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;

            var stopwatch = Stopwatch.StartNew();
            context.Items.Add("SerpentineHandlerTimer", stopwatch);
        }

        private void OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;

            var stopwatch = (Stopwatch)context.Items["SerpentineHandlerTimer"];
            stopwatch.Stop();
            var result = stopwatch.ElapsedMilliseconds;

            context.Items.Add("SerpentineHandlerTime", result);
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;

            var stopwatch = (Stopwatch)context.Items["SerpentineRequestTimer"];
            stopwatch.Stop();
            var requestElapsed = stopwatch.ElapsedMilliseconds;

            var handlerElapsed = (long) context.Items["SerpentineHandlerTime"];

            //TODO get body size, calculate min, max, average

            context.Response.AppendHeader("X-Serpentine-RequestTime", requestElapsed.ToString());
            context.Response.AppendHeader("X-Serpentine-HandlerTime", handlerElapsed.ToString());

            //Inject html
            if (context.Response.ContentType == MediaTypeNames.Text.Html)
            {
                context.Response.Write($"RequestTime: {requestElapsed}ms<br/>HandlerTime: {handlerElapsed}ms");
            }
        }

        public void Dispose()
        {
        }
    }
}
