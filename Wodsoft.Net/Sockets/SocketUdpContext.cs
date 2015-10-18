using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpContext : ISocketDgramContext
    {
        private Socket _Socket;
        public IPEndPoint LocalEndPoint { get; private set; }

        public IPEndPoint RemoteEndPoint { get; private set; }

        public SocketUdpContext(Socket socket, IPEndPoint remoteEndPoint)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint");
            _Socket = socket;
            RemoteEndPoint = remoteEndPoint;
            LocalEndPoint = (IPEndPoint)socket.LocalEndPoint;
        }

        public bool Send(byte[] data, SocketFlags flags)
        {
            return _Socket.SendTo(data, flags, RemoteEndPoint) == data.Length;
        }
        
        public async Task<bool> SendAsync(byte[] data, SocketFlags flags)
        {
            return await Task.Factory.FromAsync<int>(_Socket.BeginSendTo(data, 0, data.Length, flags, RemoteEndPoint, null, null), _Socket.EndSendTo) == data.Length;
        }

        public void OnReceive(byte[] data, int length)
        {
            if (Receive != null)
                Receive(data, length);
        }

        public void OnDisconnect()
        {
            if (Disconnect != null)
                Disconnect(this, null);
        }

        public event EventHandler Disconnect;

        public event SocketDgramReceiveDelegate Receive;
    }
}
