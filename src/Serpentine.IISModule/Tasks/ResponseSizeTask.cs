using System.Net.Mime;
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

            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-ResponseSize", sizeFilter.Length.ToString());
            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-ResponseSizeMin", ResponseSizeStorage.Instance.MinimumSize.ToString());
            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-ResponseSizeMax", ResponseSizeStorage.Instance.MaximumSize.ToString());
            _taskContext.HttpContext.Response.AppendHeader("X-Serpentine-ResponseSizeAvg", ResponseSizeStorage.Instance.AverageSize.ToString());

            //Inject html
            if (_taskContext.HttpContext.Response.ContentType == MediaTypeNames.Text.Html)
            {
                _taskContext.HttpContext.Response.Write($"ResponseSizeMin: {ResponseSizeStorage.Instance.MinimumSize} bytes<br/>");
                _taskContext.HttpContext.Response.Write($"ResponseSizeMax: {ResponseSizeStorage.Instance.MaximumSize} bytes<br/>");
                _taskContext.HttpContext.Response.Write($"ResponseSizeAvg: {ResponseSizeStorage.Instance.AverageSize} bytes<br/>");
            }
        }
    }
}
