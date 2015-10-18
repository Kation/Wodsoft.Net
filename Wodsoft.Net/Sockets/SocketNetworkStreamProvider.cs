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
    }
}
