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
            _IsConnected = true;
        }

        public SocketUdpContext Context { get; private set; }

        private bool _IsConnected;
        public override bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
        }

        public override void Disconnect()
        {
            Context.OnDisconnect();
            OnDisconnected();
            _IsConnected = false;
        }

        public override Task DisconnectAsync()
        {
            return Task.Run(() =>
            {
                Context.OnDisconnect();
                OnDisconnected();
                _IsConnected = false;
            });
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
