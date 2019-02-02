using System;

namespace Serpentine.IISModule.Tasks.Helpers
{
    internal class ResponseSizeStorage
    {
        private static readonly Lazy<ResponseSizeStorage> LazyInstance = new Lazy<ResponseSizeStorage>(() => new ResponseSizeStorage());

        public static ResponseSizeStorage Instance => LazyInstance.Value;

        public long MinimumSize { get; private set; }
        public long MaximumSize { get; private set; }
        public long AverageSize { get; private set; }

        private long _count;

        private readonly object _lock = new object();

        private ResponseSizeStorage()
        {
            MinimumSize = long.MaxValue;
            MaximumSize = long.MinValue;
            AverageSize = 0;
        }

        public void UpdateSize(long size)
        {
            lock (_lock)
            {
                if (size < MinimumSize)
                {
                    MinimumSize = size;
                }
                if (size > MaximumSize)
                {
                    MaximumSize = size;
                }

                _count++;
                AverageSize = (AverageSize * (_count - 1) + size) / _count;
            }
        }

    }
}
