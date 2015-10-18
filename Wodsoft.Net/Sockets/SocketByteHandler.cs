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

        public IAsyncResult BeginReceive(SocketStreamHandlerContext<byte[], byte[]> context, AsyncCallback callback, object state)
        {
            SocketHandlerAsyncResult<byte[], byte[]> asyncResult = new SocketHandlerAsyncResult<byte[], byte[]>(context, state);
            SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>> asyncState = new SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>>();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            asyncState.Data = context;
            context.ReceiveContext.ByteBuffer = new byte[BufferLength];
            context.Stream.BeginRead(context.ReceiveContext.ByteBuffer, 0, BufferLength, BeginReceiveCallback, asyncState);
            return asyncResult;
        }

        private void BeginReceiveCallback(IAsyncResult ar)
        {
            SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>> asyncState = (SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>>)ar.AsyncState;
            SocketHandlerAsyncResult<byte[], byte[]> asyncResult = (SocketHandlerAsyncResult<byte[], byte[]>)asyncState.AsyncResult;
            asyncResult.IsCompleted = true;
            int length = asyncState.Data.Stream.EndRead(ar);
            if (length < BufferLength)
                asyncState.Data.ReceiveContext.ByteBuffer = asyncState.Data.ReceiveContext.ByteBuffer.Take(length).ToArray();
            if (asyncState.AsyncCallback != null)
                asyncState.AsyncCallback(asyncResult);
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
        }

        public byte[] EndReceive(IAsyncResult ar)
        {
            SocketHandlerAsyncResult<byte[], byte[]> asyncResult = ar as SocketHandlerAsyncResult<byte[], byte[]>;
            if (asyncResult == null)
                throw new ArgumentNullException("异步结果错误。");
            byte[] buffer = asyncResult.Context.ReceiveContext.ByteBuffer;
            asyncResult.Context.ReceiveContext.Reset();
            return buffer;
        }

        public byte[] Receive(SocketStreamHandlerContext<byte[], byte[]> context)
        {
            context.ReceiveContext.CheckQueue();
            byte[] buffer = new byte[BufferLength];
            int length = context.Stream.Read(buffer, 0, BufferLength);
            if (length < BufferLength)
                return buffer.Take(length).ToArray();
            context.ReceiveContext.Reset();
            return buffer;
        }

        public async Task<byte[]> ReceiveAsync(SocketStreamHandlerContext<byte[], byte[]> context)
        {
            byte[] buffer = new byte[BufferLength];
            int length = await context.Stream.ReadAsync(buffer, 0, BufferLength);
            if (length < BufferLength)
                return buffer.Take(length).ToArray();
            context.ReceiveContext.Reset();
            return buffer;
        }

        public IAsyncResult BeginSend(byte[] data, SocketStreamHandlerContext<byte[], byte[]> context, AsyncCallback callback, object state)
        {
            SocketAsyncResult asyncResult = new SocketAsyncResult(state);
            SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>> asyncState = new SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>>();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            context.Stream.BeginWrite(data, 0, data.Length, BeginSendCallback, asyncState);
            return asyncResult;
        }
        
        private void BeginSendCallback(IAsyncResult ar)
        {
            SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>> asyncState = (SocketAsyncState<SocketStreamHandlerContext<byte[], byte[]>>)ar.AsyncState;
            SocketAsyncResult asyncResult = (SocketAsyncResult)asyncState.AsyncResult;
            asyncResult.IsCompleted = true;
            asyncState.Data.Stream.EndWrite(ar);
            if (asyncState.AsyncCallback != null)
                asyncState.AsyncCallback(asyncResult);
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
        }

        public bool EndSend(IAsyncResult asyncResult)
        {
            return true;
        }

        public bool Send(byte[] data, SocketStreamHandlerContext<byte[], byte[]> context)
        {
            try
            {
                context.Stream.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendAsync(byte[] data, SocketStreamHandlerContext<byte[], byte[]> context)
        {
            try
            {
                await context.Stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
