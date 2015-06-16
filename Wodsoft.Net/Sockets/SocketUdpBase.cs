using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public abstract class SocketUdpBase<TIn, TOut> : SocketBase<TIn, TOut>
    {
        protected SocketUdpBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler)
            :base(socket, socketHandler, new SocketUdpStreamProvider())
        {

        }

        public abstract override void Disconnect();

        public abstract override Task DisconnectAsync();

        public abstract override IAsyncResult BeginDisconnect(AsyncCallback callback, object state);

        public abstract override void EndDisconnect(IAsyncResult ar);

        public abstract override IPEndPoint RemoteEndPoint { get; }

        public override IPEndPoint LocalEndPoint { get { return (IPEndPoint)Socket.LocalEndPoint; } }
    }
}
