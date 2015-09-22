using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpHost<TIn, TOut>
    {
        private Dictionary<IPEndPoint, SocketUdpClient<TIn, TOut>> _Clients;
        private IPEndPoint _ReceiveFrom;

        public SocketUdpHost(ISocketDgramHandler<TIn, TOut> socketHandler)
        {
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            Handler = socketHandler;
            Timeout = TimeSpan.FromSeconds(30);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IPv4);
            BufferSize = 1024;
            _Clients = new Dictionary<IPEndPoint, SocketUdpClient<TIn, TOut>>();
        }

        public ISocketDgramHandler<TIn, TOut> Handler { get; private set; }

        protected internal Socket Socket { get; private set; }

        public int Port { get; private set; }

        public TimeSpan Timeout { get; set; }

        public bool IsStart { get; private set; }

        public int BufferSize { get; set; }

        public void Start(IPEndPoint endPoint)
        {
            IsStart = true;
            _ReceiveFrom = endPoint;
            BeginReceive(new IPEndPoint(endPoint.Address, endPoint.Port));
            Port = ((IPEndPoint)Socket.LocalEndPoint).Port;
        }

        private void BeginReceive(IPEndPoint endPoint)
        {
            byte[] buffer = new byte[BufferSize];
            EndPoint cEndPoint = endPoint;
            Socket.BeginReceiveFrom(buffer, 0, BufferSize, SocketFlags.None, ref cEndPoint, EndReceive, buffer);
        }

        private void EndReceive(IAsyncResult ar)
        {
            EndPoint endPoint = null;
            var length = Socket.EndReceiveFrom(ar, ref endPoint);
            var buffer = (byte[])ar.AsyncState;
            IPEndPoint cEndPoint = (IPEndPoint)endPoint;
            SocketUdpClient<TIn,TOut> client;
            if (!_Clients.TryGetValue(cEndPoint, out client))
            {
                client = new SocketUdpClient<TIn, TOut>(new SocketUdpContext(Socket, cEndPoint), Handler);
                _Clients.Add(cEndPoint, client);
                if (AcceptCompleted != null)
                    AcceptCompleted(this, new SocketEventArgs<ISocket<TIn, TOut>>(client, SocketAsyncOperation.Accept));
            }
            BeginReceive(new IPEndPoint(_ReceiveFrom.Address, _ReceiveFrom.Port));
            client.Context.OnReceive(buffer, length);
        }

        public void Stop()
        {

        }

        public void Open(IPEndPoint endPoint)
        {

        }

        public event EventHandler<SocketEventArgs<ISocket<TIn, TOut>>> AcceptCompleted;

    }
}
