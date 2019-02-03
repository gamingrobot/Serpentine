using NSubstitute;
using Serpentine.IISModule.Models;
using Serpentine.IISModule.Tasks;
using Serpentine.IISModule.Tasks.Helpers;
using Xunit;

namespace Serpentine.IISModule.Tests.Tasks
{
    public class RequestTimingTaskFacts
    {
        [Fact]
        public void BeginRequest_should_start_requesttimer()
        {
            var requestTimer = Substitute.For<IRequestTimer>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.BeginRequest();

            //Assert
            requestTimer.Received().StartRequestTimer();
        }

        [Fact]
        public void BeginRequest_should_reset_existing_timer()
        {
            var requestTimer = Substitute.For<IRequestTimer>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.BeginRequest();

            //Assert
            requestTimer.Received().Reset();
        }

        [Fact]
        public void PreHandler_should_start_handlertimer()
        {
            var requestTimer = Substitute.For<IRequestTimer>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.PreHandler();

            //Assert
            requestTimer.Received().StartHandlerTimer();
        }


        [Fact]
        public void PostHandler_should_stop_handlertimer()
        {
            var requestTimer = Substitute.For<IRequestTimer>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.PostHandler();

            //Assert
            requestTimer.Received().StopHandlerTimer();
        }


        [Fact]
        public void EndRequest_should_stop_requesttimer()
        {
            var requestTimer = Substitute.For<IRequestTimer>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.EndRequest();

            //Assert
            requestTimer.Received().StopRequestTimer();
        }


        [Fact]
        public void EndRequest_should_add_request_time()
        {
            long requestTime = 123;
            var requestTimer = Substitute.For<IRequestTimer>();
            requestTimer.GetRequestMilliseconds().Returns(requestTime);

            var context = Substitute.For<IMetricTaskContext>();
            var metricsResponse = Substitute.For<IMetricsResponse>();
            context.MetricsResponse.Returns(metricsResponse);

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.EndRequest();

            //Assert
            metricsResponse.Received()
                .AddMetric(Arg.Is("request-time"), Arg.Any<string>(), requestTime, Arg.Any<MetricType>());
        }

        [Fact]
        public void EndRequest_should_add_request_handler_time()
        {
            long handlerTime = 123;
            var requestTimer = Substitute.For<IRequestTimer>();
            requestTimer.GetHandlerMilliseconds().Returns(handlerTime);

            var context = Substitute.For<IMetricTaskContext>();
            var metricsResponse = Substitute.For<IMetricsResponse>();
            context.MetricsResponse.Returns(metricsResponse);

            var task = new RequestTimingTask(context, requestTimer);

            //Act
            task.EndRequest();

            //Assert
            metricsResponse.Received()
                .AddMetric(Arg.Is("request-handler-time"), Arg.Any<string>(), handlerTime, Arg.Any<MetricType>());
        }
    }
}