using System.Collections.Generic;
using System.Net.Mime;
using System.Web;

namespace Serpentine.IISModule
{
    internal interface IMetricsResponse
    {
        void AddMetric(Metric metric);
        void AddMetric(string name, string fullName, long value, string units);
        void Render(HttpResponseBase baseResponse);
    }

    internal class MetricsResponse : IMetricsResponse
    {
        private readonly IList<Metric> _metrics;

        public MetricsResponse()
        {
            _metrics = new List<Metric>();
        }

        public void AddMetric(Metric metric)
        {
            _metrics.Add(metric);
        }

        public void AddMetric(string name, string fullName, long value, string units)
        {
            AddMetric(new Metric
            {
                Name = name,
                FullName = fullName,
                Value = value,
                Units = units
            });
        }

        public void Render(HttpResponseBase baseResponse)
        {
            foreach (var metric in _metrics)
            {
                //Set header
                baseResponse.AppendHeader($"x-serpentine-{metric.Name}", metric.Value.ToString());

                //Inject html
                if (baseResponse.ContentType == MediaTypeNames.Text.Html)
                {
                    baseResponse.Write($"{metric.FullName}: {metric.Value}{metric.Units}<br/>");
                }
            }
        }
    }
}