using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Http
{
    public class HttpInputStream : SocketBufferedStream
    {
        private Stream _Stream;
        private int _Length;

        public HttpInputStream(Stream networkStream, int length)
        {
            if (networkStream == null)
                throw new ArgumentNullException("networkStream");
            _Stream = networkStream;
            _Length = length;
        }

        public HttpInputStream(Stream networkStream, int length, byte[] bufferedData)
            : this(networkStream, length)
        {
            if (bufferedData == null)
                throw new ArgumentNullException("bufferedData");
            base.Write(bufferedData, 0, bufferedData.Length);
        }

        public HttpInputStream(Stream networkStream, byte[] bufferedData)
            : this(networkStream, -1)
        {
            if (bufferedData == null)
                throw new ArgumentNullException("bufferedData");
            base.Write(bufferedData, 0, bufferedData.Length);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                if (_Length == -1)
                    throw new NotSupportedException();
                return _Length;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int length = base.Read(buffer, offset, count);
            if (length == count || length != -1)
                return length;
            int index = offset + length;
            int size = count - length;
            while (size > 0)
            {
                index += _Stream.Read(buffer, index, size);
                size = count - index - offset;
            }
            base.Write(buffer, length, count - length - offset);
            return count;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            int length = base.Read(buffer, offset, count);
            if (length == count || length != -1)
                return length;
            int index = offset + length;
            int size = count - length;
            while (size > 0)
            {
                index += await _Stream.ReadAsync(buffer, index, size);
                size = count - index - offset;
            }
            base.Write(buffer, length, count - length - offset);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current:
                    offset += base.Position;
                    if (offset < 0)
                        offset = 0;
                    else if (offset > base.Length)
                        if (_Length == -1)
                            offset = base.Length;
                        else
                            if (offset > _Length)
                                offset = _Length;
                    break;
                case SeekOrigin.End:
                    if (_Length == -1)
                        throw new NotSupportedException("当前流不支持后置寻址。");
                    offset += _Length;
                    if (offset < 0)
                        offset = 0;
                    else if (offset > _Length)
                        offset = _Length;
                    break;
            }
            Position = offset;
            return Position;
        }

        public override long Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > base.Length)
                    if (_Length == -1)
                        value = base.Length;
                    else
                        if (value > _Length)
                            value = _Length;
                if (value > base.Length)
                {
                    byte[] data = new byte[value - base.Length];
                    int length = 0;
                    while (length < data.Length)
                        length += _Stream.Read(data, length, data.Length - length);
                }
                base.Position = value;

            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            _Stream = null;
            base.Close();
        }
    }
}
