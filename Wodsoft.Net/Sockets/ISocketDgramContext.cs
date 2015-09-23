using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public interface ISocketDgramContext
    {
        bool Send(byte[] data, SocketFlags flags);

        IAsyncResult BeginSend(byte[] data, SocketFlags flags, AsyncCallback callback, object state);

        bool EndSend(IAsyncResult ar);

        event SocketDgramReceiveDelegate Receive;
    }

    public delegate void SocketDgramReceiveDelegate(byte[] data, int length);
}
