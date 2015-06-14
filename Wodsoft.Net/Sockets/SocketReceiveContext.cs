using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketReceiveContext<T> : SocketProcessContext
    {
        public SocketReceiveContext()
        {
            Buffer = new SocketBufferedStream();
        }

        public SocketBufferedStream Buffer { get; private set; }

        public byte[] ByteBuffer { get; set; }

        public T Result { get; set; }

        public override void Reset()
        {
            Result = default(T);
            Buffer.ResetPosition();
            ByteBuffer = null;
            base.Reset();
        }
    }
}
