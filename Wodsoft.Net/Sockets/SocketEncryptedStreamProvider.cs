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
    public class SocketEncryptedStreamProvider : ISocketStreamProvider
    {
        public SocketEncryptedStreamProvider(bool isServer)
        {
            IsServer = isServer;
        }

        public bool IsServer { get; set; }

        public Stream GetStream(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            var stream = new SocketEncryptedStream(new NetworkStream(socket), 214);
            if (IsServer)
                stream.AuthenticateAsServer();
            else
                stream.AuthenticateAsClient();
            return stream;
        }

        public async Task<Stream> GetStreamAsync(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            var stream = new SocketEncryptedStream(new NetworkStream(socket), 214);
            if (IsServer)
                await stream.AuthenticateAsServerAsync();
            else
                await stream.AuthenticateAsClientAsync();
            return stream;
        }
    }
}
