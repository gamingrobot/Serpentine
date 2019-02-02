using System.Diagnostics;

namespace Serpentine.IISModule
{
    internal class RequestTimer
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

    }
}
