using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketNetworkStreamProvider : ISocketStreamProvider
    {
        public Stream GetStream(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            return new NetworkStream(socket);
        }

        public Task<Stream> GetStreamAsync(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            return Task.Run<Stream>(() =>
            {
                return new NetworkStream(socket);
            });
        }

        public IAsyncResult BeginGetStream(Socket socket, AsyncCallback callback, object state)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            SocketAsyncResult<Stream> asyncResult = new SocketAsyncResult<Stream>(state);
            asyncResult.IsCompleted = true;
            asyncResult.CompletedSynchronously = true;
            asyncResult.Data = new NetworkStream(socket);
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            if (callback != null)
                callback(asyncResult);
            return asyncResult;
        }

        public Stream EndGetStream(IAsyncResult ar)
        {
            SocketAsyncResult<Stream> asyncResult = (SocketAsyncResult<Stream>)ar;
            if (asyncResult == null)
                throw new ArgumentException("异步结果异常。");
            return asyncResult.Data;
        }
    }
}
