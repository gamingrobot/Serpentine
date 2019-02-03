using System.IO;
using FluentAssertions;
using NSubstitute;
using Serpentine.IISModule.Tasks;
using Serpentine.IISModule.Tasks.Helpers;
using Xunit;

namespace Serpentine.IISModule.Tests.Tasks
{
    public class ResponseSizeTaskFacts
    {
        [Fact]
        public void BeginRequest_should_create_filter()
        {
            var storage = Substitute.For<IResponseSizeStorage>();
            var context = Substitute.For<IMetricTaskContext>();

            var task = new ResponseSizeTask(context, storage);

            //Act
            task.BeginRequest();

            //Assert
            context.HttpContext.Response.Filter.Should().BeOfType<ResponseSizeFilter>();
        }

        [Fact]
        public void EndRequest_should_add_request_size()
        {
            var dummyBytes = System.Text.Encoding.UTF8.GetBytes("Hello World");
            var fakeStream = new MemoryStream(dummyBytes);

            var storage = Substitute.For<IResponseSizeStorage>();
            var context = Substitute.For<IMetricTaskContext>();
            context.HttpContext.Response.Filter.Returns(fakeStream);
            var metricsResponse = Substitute.For<IMetricsResponse>();
            context.MetricsResponse.Returns(metricsResponse);

            var task = new ResponseSizeTask(context, storage);

            //Act
            task.EndRequest();

            //Assert
            metricsResponse.Received()
                .AddMetric(Arg.Is("response-size"), Arg.Any<string>(), dummyBytes.Length, Arg.Any<string>());
        }
    }
}