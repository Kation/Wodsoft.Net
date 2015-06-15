using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Wodsoft.Net.Sockets
{
    public class SocketAsyncState
    {
        public AsyncCallback AsyncCallback { get; set; }

        public IAsyncResult AsyncResult { get; set; }
    }

    public class SocketAsyncState<T> : SocketAsyncState
    {
        public T Data { get; set; }
    }
}
