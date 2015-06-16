using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpListener<TIn, TOut>
    {
        public SocketUdpListener(ISocketHandler<TIn, TOut> socketHandler)
        {
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            Handler = socketHandler;
            Timeout = TimeSpan.FromSeconds(30);
        }

        public ISocketHandler<TIn, TOut> Handler { get; private set; }

        public int Port { get; set; }

        public TimeSpan Timeout { get; set; }

        public void Start()
        {

        }
    }
}
