using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public interface ISocketStreamProvider
    {
        Stream GetStream(Socket socket);
        Task<Stream> GetStreamAsync(Socket socket);
        IAsyncResult BeginGetStream(Socket socket, AsyncCallback callback, object state);
        Stream EndGetStream(IAsyncResult ar);
    }
}
