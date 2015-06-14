using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketNetworkStreamProvider : ISocketStreamProvider
    {
        public Stream GetStream(Socket socket)
        {
            return new NetworkStream(socket);
        }

        public Task<Stream> GetStreamAsync(Socket socket)
        {
            return Task.Run<Stream>(() =>
            {
                return new NetworkStream(socket);
            });
        }

        public IAsyncResult BeginGetStream(Socket socket, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Stream EndGetStream(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }
    }
}
