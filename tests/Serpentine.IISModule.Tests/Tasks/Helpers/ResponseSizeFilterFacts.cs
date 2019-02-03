using System.IO;
using System.Text;
using FluentAssertions;
using Serpentine.IISModule.Tasks.Helpers;
using Xunit;

namespace Serpentine.IISModule.Tests.Tasks.Helpers
{
    public class ResponseSizeFilterFacts
    {
        [Fact]
        public void ResponseSizeFilter_correctly_counts_size()
        {
            var dummyStream = new MemoryStream();
            var filter = new ResponseSizeFilter(dummyStream);

            var dummyInput = Encoding.UTF8.GetBytes("DummyString");

            //Act
            filter.Write(dummyInput, 0, dummyInput.Length);

            //Assert
            filter.Length.Should().Be(dummyInput.Length);
        }

        [Fact]
        public void ResponseSizeFilter_acts_as_passthrough()
        {
            var dummyStream = new MemoryStream();
            var filter = new ResponseSizeFilter(dummyStream);

            var dummyInput = Encoding.UTF8.GetBytes("DummyString");

            //Act
            filter.Write(dummyInput, 0, dummyInput.Length);

            //Assert
            dummyStream.Length.Should().Be(dummyInput.Length);
        }

        [Fact]
        public void ResponseSizeFilter_handles_offset_correctly()
        {
            var dummyStream = new MemoryStream();
            var filter = new ResponseSizeFilter(dummyStream);

            var dummyInput = Encoding.UTF8.GetBytes("DummyString");
            var offset = 5;

            var correctLength = dummyInput.Length - offset;

            //Act
            filter.Write(dummyInput, offset, correctLength);

            //Assert
            filter.Length.Should().Be(correctLength);
            dummyStream.Length.Should().Be(correctLength);
        }
    }
}
