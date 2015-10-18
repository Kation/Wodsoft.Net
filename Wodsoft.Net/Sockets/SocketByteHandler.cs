using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketByteHandler : ISocketStreamHandler<byte[], byte[]>
    {
        public int BufferLength { get; private set; }

        public SocketByteHandler() : this(4096) { }

        public SocketByteHandler(int bufferLength)
        {
            if (bufferLength < 1)
                throw new ArgumentOutOfRangeException("bufferLength", "缓冲长度不能小于1.");
            BufferLength = bufferLength;
        }

        public byte[] Receive(SocketStreamHandlerContext<byte[], byte[]> context)
        {
            context.ReceiveContext.CheckQueue();
            try
            {
                byte[] buffer = new byte[BufferLength];
                int length = context.Stream.Read(buffer, 0, BufferLength);
                if (length < BufferLength)
                    return buffer.Take(length).ToArray();
                return buffer;
            }
            catch
            {
                return null;
            }
            finally
            {
                context.ReceiveContext.Reset();
            }
        }

        public async Task<byte[]> ReceiveAsync(SocketStreamHandlerContext<byte[], byte[]> context)
        {
            context.ReceiveContext.CheckQueue();
            try
            {
                byte[] buffer = new byte[BufferLength];
                int length = await context.Stream.ReadAsync(buffer, 0, BufferLength);
                if (length < BufferLength)
                    return buffer.Take(length).ToArray();
                context.ReceiveContext.Reset();
                return buffer;
            }
            catch
            {
                return null;
            }
            finally
            {
                context.ReceiveContext.Reset();
            }
        }

        public bool Send(byte[] data, SocketStreamHandlerContext<byte[], byte[]> context)
        {
            context.SendContext.CheckQueue();
            try
            {
                context.Stream.Write(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
        }

        public async Task<bool> SendAsync(byte[] data, SocketStreamHandlerContext<byte[], byte[]> context)
        {
            context.SendContext.CheckQueue();
            try
            {
                await context.Stream.WriteAsync(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
        }
    }
}
