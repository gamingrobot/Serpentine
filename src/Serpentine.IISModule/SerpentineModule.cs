using System;
using System.Collections.Generic;
using System.Web;
using Serpentine.IISModule.Tasks;

namespace Serpentine.IISModule
{
    public class SerpentineModule : IHttpModule
    {
        private IList<IMetricsTask> _metricsTasks;

        public void Init(HttpApplication context)
        {
            _metricsTasks = new List<IMetricsTask>
            {
                new RequestTimingTask(context),
                new ResponseSizeTask(context)
            };

            context.BeginRequest += OnBeginRequest;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
            context.EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            foreach (var task in _metricsTasks)
            {
                task.BeginRequest();
            }
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            foreach (var task in _metricsTasks)
            {
                task.PreHandler();
            }
        }

        private void OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            foreach (var task in _metricsTasks)
            {
                task.PostHandler();
            }
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            foreach (var task in _metricsTasks)
            {
                task.EndRequest();
            }
        }

        public void Dispose()
        {
        }
    }
}
