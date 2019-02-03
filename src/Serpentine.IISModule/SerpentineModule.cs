using System;
using System.Web;

namespace Serpentine.IISModule
{
    public class SerpentineModule : IHttpModule
    {
        private const string TaskManagerKey = "SerpentineMetricsTaskManager";

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecute;
            context.EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HandleEvent(sender, (manager, context) => manager.BeginRequest(context));
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            HandleEvent(sender, (task, context) => task.PreHandler(context));
        }

        private void OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            HandleEvent(sender, (task, context) => task.PostHandler(context));
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            HandleEvent(sender, (task, context) => task.EndRequest(context));
        }

        private void HandleEvent(object sender, Action<IMetricsEventHandler, HttpContextBase> handler)
        {
            try
            {
                var application = (HttpApplication) sender;
                var wrapper = new HttpContextWrapper(application.Context);
                IMetricsEventHandler metricsEventHandler;
                if (wrapper.Items.Contains(TaskManagerKey))
                {
                    metricsEventHandler = (IMetricsEventHandler) wrapper.Items[TaskManagerKey];
                }
                else
                {
                    var taskContext = new MetricTaskContext(ApplicationStorage.Instance, new MetricsResponse());
                    metricsEventHandler = new MetricsEventHandler(taskContext);
                    wrapper.Items[TaskManagerKey] = metricsEventHandler;
                }

                handler(metricsEventHandler, wrapper);
            }
            catch (Exception)
            {
                //TODO: log failure
                //Fail quietly (we don't want to disrupt the application)
            }
        }

        public void Dispose()
        {
        }
    }
}