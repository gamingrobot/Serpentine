using System.Diagnostics;

namespace Serpentine.IISModule.Tasks.Helpers
{
    internal interface IRequestTimer
    {
        void StartRequestTimer();
        void StopRequestTimer();
        void StartHandlerTimer();
        void StopHandlerTimer();
        long GetRequestMilliseconds();
        long GetHandlerMilliseconds();
        void Reset();
    }

    internal class RequestTimer : IRequestTimer
    {
        private readonly Stopwatch _requestStopwatch;
        private readonly Stopwatch _handlerStopwatch;

        public RequestTimer()
        {
            _requestStopwatch = new Stopwatch();
            _handlerStopwatch = new Stopwatch();
        }

        public void StartRequestTimer()
        {
            _requestStopwatch.Start();
        }

        public void StopRequestTimer()
        {
            _requestStopwatch.Stop();
        }

        public void StartHandlerTimer()
        {
            _handlerStopwatch.Start();
        }

        public void StopHandlerTimer()
        {
            _handlerStopwatch.Stop();
        }

        public long GetRequestMilliseconds()
        {
            return _requestStopwatch.ElapsedMilliseconds;
        }

        public long GetHandlerMilliseconds()
        {
            return _handlerStopwatch.ElapsedMilliseconds;
        }

        public void Reset()
        {
            _requestStopwatch.Reset();
            _handlerStopwatch.Reset();
        }
    }
}