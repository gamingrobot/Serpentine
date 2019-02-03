using System.Net.Mime;
using System.Web;
using NSubstitute;
using Xunit;

namespace Serpentine.IISModule.Tests
{
    public class MetricsEventHandlerFacts
    {
        [Fact]
        public void MetricsEventHandler_handler_flow()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Response.ContentType.Returns(MediaTypeNames.Text.Html);
            var storage = new ApplicationStorageMock();
            var response = new MetricsResponse();
            var context = new MetricTaskContext(storage, response);

            var handler = new MetricsEventHandler(context);

            //Act
            handler.BeginRequest(httpContext);
            handler.PreHandler(httpContext);
            handler.PostHandler(httpContext);
            handler.EndRequest(httpContext);

            //Assert
            httpContext.Response.Received().AppendHeader(Arg.Any<string>(), Arg.Any<string>());
            httpContext.Response.Received().Write(Arg.Any<string>());
        }

        [Fact]
        public void MetricsEventHandler_handler_doesnt_write_to_body_if_not_html()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Response.ContentType.Returns(MediaTypeNames.Text.Xml);
            var storage = new ApplicationStorageMock();
            var response = new MetricsResponse();
            var context = new MetricTaskContext(storage, response);

            var handler = new MetricsEventHandler(context);

            //Act
            handler.BeginRequest(httpContext);
            handler.PreHandler(httpContext);
            handler.PostHandler(httpContext);
            handler.EndRequest(httpContext);

            //Assert
            httpContext.Response.Received().AppendHeader(Arg.Any<string>(), Arg.Any<string>());
            httpContext.Response.DidNotReceive().Write(Arg.Any<string>());
        }
    }
}