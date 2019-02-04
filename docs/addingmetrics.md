# Adding a new metric

1. Implement `IMetricTask`
2. Add Implementation to MetricsEventHandler
3. Call `IMetricsTaskContext.MetricsResponse.AddMetric` in MetricTask to add an additional metric