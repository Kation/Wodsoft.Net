using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketHandlerContext<TIn, TOut>
    {
        public SocketHandlerContext(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            Stream = stream;
            ReceiveContext = new SocketReceiveContext<TOut>(stream);
            SendContext = new SocketSendContext<TIn>(stream);
        }

        public Stream Stream { get; private set; }

        public SocketReceiveContext<TOut> ReceiveContext { get; private set; }

        public SocketSendContext<TIn> SendContext { get; private set; }
    }
}
