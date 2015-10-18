using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolSession
    {
        public ProtocolSession(ISocket<byte[], byte[]> socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            Socket = socket;
        }

        public ISocket<byte[], byte[]> Socket { get; private set; }

        private SocketDataBag _DataBag;
        public dynamic DataBag
        {
            get
            {
                if (_DataBag == null)
                    _DataBag = new SocketDataBag();
                return _DataBag;
            }
        }
    }
}
