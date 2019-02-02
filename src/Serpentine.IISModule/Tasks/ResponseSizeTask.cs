using System.Net.Mime;
using System.Web;
using Serpentine.IISModule.Tasks.Helpers;

namespace Serpentine.IISModule.Tasks
{
    internal class ResponseSizeTask : IMetricsTask
    {
        private readonly HttpApplication _application;

        public ResponseSizeTask(HttpApplication application)
        {
            _application = application;
        }

        public void BeginRequest()
        {
            _application.Context.Response.Filter = new ResponseSizeFilter(_application.Context.Response.Filter);
        }

        public void PreHandler()
        {
        }

        public void PostHandler()
        {
        }

        public void EndRequest()
        {
            var sizeFilter = _application.Context.Response.Filter;
            ResponseSizeStorage.Instance.UpdateSize(sizeFilter.Length);

            _application.Context.Response.AppendHeader("X-Serpentine-RequestSize", sizeFilter.Length.ToString());
            _application.Context.Response.AppendHeader("X-Serpentine-RequestSizeMin", ResponseSizeStorage.Instance.MinimumSize.ToString());
            _application.Context.Response.AppendHeader("X-Serpentine-RequestSizeMax", ResponseSizeStorage.Instance.MaximumSize.ToString());
            _application.Context.Response.AppendHeader("X-Serpentine-RequestSizeAvg", ResponseSizeStorage.Instance.AverageSize.ToString());

            //Inject html
            if (_application.Context.Response.ContentType == MediaTypeNames.Text.Html)
            {
                _application.Context.Response.Write($"RequestSizeMin: {ResponseSizeStorage.Instance.MinimumSize} bytes<br/>");
                _application.Context.Response.Write($"RequestSizeMax: {ResponseSizeStorage.Instance.MaximumSize} bytes<br/>");
                _application.Context.Response.Write($"RequestSizeAvg: {ResponseSizeStorage.Instance.AverageSize} bytes<br/>");
            }
        }
    }
}
