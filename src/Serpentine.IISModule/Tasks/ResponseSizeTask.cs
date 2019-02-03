using Serpentine.IISModule.Tasks.Helpers;

namespace Serpentine.IISModule.Tasks
{
    internal class ResponseSizeTask : IMetricTask
    {
        private readonly IResponseSizeStorage _storage;
        private readonly IMetricTaskContext _taskContext;

        public ResponseSizeTask(IMetricTaskContext taskContext) : this(taskContext, ResponseSizeStorage.Instance)
        {
        }

        internal ResponseSizeTask(IMetricTaskContext taskContext, IResponseSizeStorage storage)
        {
            _taskContext = taskContext;
            _storage = storage;
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
            _storage.RecalculateSizes(sizeFilter.Length);

            _taskContext.MetricsResponse.AddMetric("response-size", "Response Size", sizeFilter.Length, "bytes");
            _taskContext.MetricsResponse.AddMetric("response-size-min", "Response Size Min", _storage.MinimumSize, "bytes");
            _taskContext.MetricsResponse.AddMetric("response-size-max", "Response Size Max", _storage.MaximumSize, "bytes");
            _taskContext.MetricsResponse.AddMetric("response-size-avg", "Response Size Avg", _storage.AverageSize, "bytes");
        }
    }
}
