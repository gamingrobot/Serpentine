namespace Serpentine.IISModule.Models
{
    internal class Metric
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public long Value { get; set; }
        public MetricType Type { get; set; }
    }
}