using System.IO;

namespace Serpentine.IISModule.Tasks.Helpers
{
    internal class ResponseSizeFilter : Stream
    {
        private readonly Stream _baseStream;
        private int _size;

        public ResponseSizeFilter(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _size += count;
            _baseStream.Write(buffer, offset, count);
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _size;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }
    }
}