namespace Serpentine.IISModule.Models
{
    internal class Metric
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public long Value { get; set; }
        public MetricType Type { get; set; }
    }
}