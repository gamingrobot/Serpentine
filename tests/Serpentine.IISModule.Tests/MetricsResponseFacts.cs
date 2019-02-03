using System.Net.Mime;
using System.Web;
using AutoFixture;
using NSubstitute;
using Serpentine.IISModule.Models;
using Xunit;

namespace Serpentine.IISModule.Tests
{
    public class MetricsResponseFacts
    {
        [Fact]
        public void Render_should_add_size_headers_to_response()
        {
            var httpResponse = Substitute.For<HttpResponseBase>();

            var fixture = new Fixture();
            var metric = fixture.Build<Metric>().With(x => x.Type, MetricType.Size).Create();

            var response = new MetricsResponse();
            response.AddMetric(metric);

            //Act
            response.Render(httpResponse);

            //Assert
            httpResponse.Received()
                .AppendHeader(Arg.Is<string>(x => x.EndsWith(metric.Name)), Arg.Is(metric.Value.ToString()));
        }

        [Fact]
        public void Render_should_add_duration_headers_to_response()
        {
            var httpResponse = Substitute.For<HttpResponseBase>();

            var fixture = new Fixture();
            var metric = fixture.Build<Metric>().With(x => x.Type, MetricType.Duration).Create();

            var response = new MetricsResponse();
            response.AddMetric(metric);

            //Act
            response.Render(httpResponse);

            //Assert
            httpResponse.Received()
                .AppendHeader(Arg.Is("Server-Timing"), Arg.Is<string>(x => x.EndsWith(metric.Value.ToString())));
        }

        [Fact]
        public void Render_should_add_html_on_html_response()
        {
            var httpResponse = Substitute.For<HttpResponseBase>();
            httpResponse.ContentType.Returns(MediaTypeNames.Text.Html);

            var fixture = new Fixture();
            var metric = fixture.Create<Metric>();

            var response = new MetricsResponse();
            response.AddMetric(metric);

            //Act
            response.Render(httpResponse);

            //Assert
            httpResponse.Received().Write(Arg.Is<string>(x =>
                x.Contains(metric.FullName) &&
                x.Contains(metric.Value.ToString())
            ));
        }

        [Fact]
        public void Render_should_not_add_html_if_not_html_response()
        {
            var httpResponse = Substitute.For<HttpResponseBase>();
            httpResponse.ContentType.Returns(MediaTypeNames.Text.Xml);

            var fixture = new Fixture();
            var metric = fixture.Create<Metric>();

            var response = new MetricsResponse();
            response.AddMetric(metric);

            //Act
            response.Render(httpResponse);

            //Assert
            httpResponse.DidNotReceive().Write(Arg.Any<string>());
        }
    }
}