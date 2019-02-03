using System;

namespace Serpentine.IISModule.Tasks.Helpers
{
    internal interface IResponseSizeStorage
    {
        long MinimumSize { get; }
        long MaximumSize { get; }
        long AverageSize { get; }
        void RecalculateSizes(long size);
    }

    //Singleton because multiple httpapplications can exist per worker process
    internal class ResponseSizeStorage : IResponseSizeStorage
    {
        private static readonly Lazy<ResponseSizeStorage> LazyInstance =
            new Lazy<ResponseSizeStorage>(() => new ResponseSizeStorage());

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

        public void RecalculateSizes(long size)
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