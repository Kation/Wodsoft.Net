using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolManager
    {
        ProtocolManager(ISocket<Package,Package> socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
        }

        //public Task Send(Package package)
        //{

        //}

        //public Task<T> SendAndReceive(Package package)
        //{

        //}

        //public Task<T> SendAndReceive(Package package, int timeout)
        //{

        //}
    }
}
