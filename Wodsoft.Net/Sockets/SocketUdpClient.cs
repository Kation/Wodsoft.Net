using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpClient<TIn, TOut> : SocketDgramBase<TIn, TOut>
    {
        public SocketUdpClient(SocketUdpContext context, ISocketDgramHandler<TIn, TOut> socketHandler)
            : base(context, socketHandler)
        {
            Context = context;
        }

        public SocketUdpContext Context { get; private set; }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void EndDisconnect(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public override IPEndPoint RemoteEndPoint
        {
            get { return Context.RemoteEndPoint; }
        }

        public override IPEndPoint LocalEndPoint
        {
            get { return Context.LocalEndPoint; }
        }
    }
}
