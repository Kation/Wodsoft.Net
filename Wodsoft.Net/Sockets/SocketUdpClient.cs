using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpClient<TIn, TOut> : SocketUdpBase<TIn, TOut>
    {
        public SocketUdpClient(ISocketHandler<TIn,TOut> socketHandler):
            base(new Socket( SocketType.Dgram, ProtocolType.Udp), socketHandler)
        {
            
        }
    }
}
