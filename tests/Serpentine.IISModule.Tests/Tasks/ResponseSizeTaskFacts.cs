using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Serpentine.IISModule.Models;
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
            var context = Substitute.For<IMetricTaskContext>();

            var task = new ResponseSizeTask(context);

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

            var context = Substitute.For<IMetricTaskContext>();
            context.HttpContext.Response.Filter.Returns(fakeStream);
            var metricsResponse = Substitute.For<IMetricsResponse>();
            context.MetricsResponse.Returns(metricsResponse);

            var task = new ResponseSizeTask(context);

            //Act
            task.EndRequest();

            //Assert
            metricsResponse.Received()
                .AddMetric(Arg.Is("response-size"), Arg.Any<string>(), dummyBytes.Length, Arg.Any<MetricType>());
        }

        [Fact]
        public void ResponseSizeTask_creates_response_size()
        {
            var storage = new ApplicationStorageMock();
            var context = Substitute.For<IMetricTaskContext>();
            context.ApplicationStorage.Returns(storage);

            var dummyStream = new DummyStream();
            dummyStream.SetLength(100);
            context.HttpContext.Response.Filter.Returns(dummyStream);

            var task = new ResponseSizeTask(context);

            //Act
            task.EndRequest();

            //Assert
            storage.Get<ResponseSizeTask.ResponseSize>(nameof(ResponseSizeTask)).Should()
                .BeOfType<ResponseSizeTask.ResponseSize>();
        }


        [Fact]
        public void ResponseSizeTask_updates_minimum()
        {
            var fixture = new Fixture();
            var fakeMetric = fixture.Build<ResponseSizeTask.ResponseSize>().With(x => x.MinimumSize, 200).Create();

            var storage = new ApplicationStorageMock();
            storage.Set(nameof(ResponseSizeTask), fakeMetric);

            var context = Substitute.For<IMetricTaskContext>();
            context.ApplicationStorage.Returns(storage);

            var dummyStream = new DummyStream();
            dummyStream.SetLength(100);
            context.HttpContext.Response.Filter.Returns(dummyStream);

            var task = new ResponseSizeTask(context);

            //Act
            task.EndRequest();

            //Assert
            storage.Get<ResponseSizeTask.ResponseSize>(nameof(ResponseSizeTask)).MinimumSize.Should().Be(100);
        }

        [Fact]
        public void ResponseSizeTask_updates_maximum()
        {
            var fixture = new Fixture();
            var fakeMetric = fixture.Build<ResponseSizeTask.ResponseSize>().With(x => x.MaximumSize, 200).Create();

            var storage = new ApplicationStorageMock();
            storage.Set(nameof(ResponseSizeTask), fakeMetric);

            var context = Substitute.For<IMetricTaskContext>();
            context.ApplicationStorage.Returns(storage);

            var dummyStream = new DummyStream();
            dummyStream.SetLength(300);
            context.HttpContext.Response.Filter.Returns(dummyStream);

            var task = new ResponseSizeTask(context);

            //Act
            task.EndRequest();

            //Assert
            storage.Get<ResponseSizeTask.ResponseSize>(nameof(ResponseSizeTask)).MaximumSize.Should().Be(300);
        }

        [Fact]
        public void ResponseSizeTask_correctly_calculates_average()
        {
            var storage = new ApplicationStorageMock();

            var context = Substitute.For<IMetricTaskContext>();
            context.ApplicationStorage.Returns(storage);

            var dummyStream = new DummyStream();
            context.HttpContext.Response.Filter.Returns(dummyStream);

            var fixture = new Fixture();
            var requests = fixture.CreateMany<long>().ToList();

            var task = new ResponseSizeTask(context);

            //Act
            foreach (var r in requests)
            {
                dummyStream.SetLength(r);
                task.EndRequest();
            }

            //Assert
            var correctAvg = requests.Sum() / (double) requests.Count;
            var assertAverage = storage.Get<ResponseSizeTask.ResponseSize>(nameof(ResponseSizeTask)).AverageSize;
            assertAverage.Should().Be(correctAvg);
        }

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        private class DummyStream : Stream
        {
            private long _length;

            public override void Flush()
            {
                throw new System.NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new System.NotImplementedException();
            }

            public override void SetLength(long value)
            {
                _length = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }

            public override bool CanRead { get; }
            public override bool CanSeek { get; }
            public override bool CanWrite { get; }

            public override long Length => _length;

            public override long Position { get; set; }
        }
    }
}