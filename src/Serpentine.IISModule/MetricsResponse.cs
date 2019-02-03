using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Web;
using Serpentine.IISModule.Models;

namespace Serpentine.IISModule
{
    internal interface IMetricsResponse
    {
        void AddMetric(Metric metric);
        void AddMetric(string name, string fullName, long value, MetricType type);
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

        public void AddMetric(string name, string fullName, long value, MetricType type)
        {
            AddMetric(new Metric
            {
                Name = name,
                FullName = fullName,
                Value = value,
                Type = type
            });
        }

        public void Render(HttpResponseBase response)
        {

            RenderHeaders(response);
            if (response.ContentType == MediaTypeNames.Text.Html)
            {
                RenderHtml(response);
            }
               
        }

        private void RenderHeaders(HttpResponseBase response)
        {
            foreach (var metric in _metrics)
            {
                if (metric.Type == MetricType.Duration)
                {
                    response.AppendHeader("Server-Timing", $"{metric.Name};dur={metric.Value}");
                }
                else
                {
                    response.AppendHeader($"x-serpentine-{metric.Name}", $"{metric.Value}");
                }
            }
        }

        private void RenderHtml(HttpResponseBase response)
        {
            response.Write($"<!--{Environment.NewLine}");
            foreach (var metric in _metrics)
            {
                response.Write($"{metric.FullName}: {metric.Value} {GetUnits(metric.Type)}{Environment.NewLine}");
            }
            response.Write("--!>");
        }

        private string GetUnits(MetricType type)
        {
            switch (type)
            {
                case MetricType.Duration:
                    return "ms";
                case MetricType.Size:
                    return "bytes";
                default:
                    return null;
            }
        }
    }
}