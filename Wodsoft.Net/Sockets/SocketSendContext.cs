using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketSendContext<T> : SocketProcessContext
    {
        public T Data { get; set; }

        public override void Reset()
        {
            Data = default(T);
            base.Reset();
        }
    }
}
