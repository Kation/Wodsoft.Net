using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketSendContext<T> : SocketProcessContext
    {
        public SocketSendContext(Stream source)
            : base(source) { }

        public T Data { get; set; }

        public override void Reset()
        {
            Data = default(T);
            base.Reset();
        }
    }
}
