# Writing a new MetricTask

1. Implement `IMetricTask`
2. Add Implementation to MetricsEventHandler
3. Use `IMetricsTaskContext.MetricsResponse.AddMetric` in MetricTask to add an additional metric
4. Use `IMetricsTaskContext.ApplicationStorage` to store data for the lifetime of the IIS Worker Process
5. Use `IMetricsTaskContext.HttpContext` to access information about the current request in the event handlers