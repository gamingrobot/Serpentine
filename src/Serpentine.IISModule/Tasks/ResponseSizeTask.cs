using System;
using Serpentine.IISModule.Tasks.Helpers;

namespace Serpentine.IISModule.Tasks
{
    internal class ResponseSizeTask : IMetricTask
    {
        private readonly IMetricTaskContext _taskContext;

        //We want to lock across all threads in a worker process
        private static readonly object Locker = new object();

        public ResponseSizeTask(IMetricTaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        public void BeginRequest()
        {
            _taskContext.HttpContext.Response.Filter = new ResponseSizeFilter(_taskContext.HttpContext.Response.Filter);
        }

        public void PreHandler()
        {
        }

        public void PostHandler()
        {
        }

        public void EndRequest()
        {
            var sizeFilter = _taskContext.HttpContext.Response.Filter;
            var result = UpdateResponseSizes(sizeFilter.Length);

            _taskContext.MetricsResponse
                .AddMetric("response-size", "Response Size", sizeFilter.Length, "bytes");
            _taskContext.MetricsResponse
                .AddMetric("response-size-min", "Response Size Min", result.MinimumSize, "bytes");
            _taskContext.MetricsResponse
                .AddMetric("response-size-max", "Response Size Max", result.MaximumSize, "bytes");
            _taskContext.MetricsResponse
                .AddMetric("response-size-avg", "Response Size Avg", Convert.ToInt64(result.AverageSize), "bytes");
        }

        private ResponseSize UpdateResponseSizes(long responseSize)
        {
            lock (Locker)
            {
                var currentState = _taskContext.ApplicationStorage.Get<ResponseSize>(nameof(ResponseSizeTask));
                if (currentState == null)
                {
                    currentState = new ResponseSize
                    {
                        MinimumSize = long.MaxValue,
                        MaximumSize = long.MinValue,
                        AverageSize = 0,
                        Count = 0
                    };
                }

                if (responseSize < currentState.MinimumSize)
                {
                    currentState.MinimumSize = responseSize;
                }

                if (responseSize > currentState.MaximumSize)
                {
                    currentState.MaximumSize = responseSize;
                }

                currentState.Count++;
                var newSum = (currentState.AverageSize * (currentState.Count - 1) + responseSize);
                currentState.AverageSize =  newSum / currentState.Count;

                _taskContext.ApplicationStorage.Set(nameof(ResponseSizeTask), currentState);

                return currentState;
            }
        }

        internal class ResponseSize
        {
            public long MinimumSize { get; set; }
            public long MaximumSize { get; set; }
            public double AverageSize { get; set; }
            public long Count { get; set; }
        }
    }
}