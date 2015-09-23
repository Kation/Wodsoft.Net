using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketSourceHandler : ISocketDgramHandler<byte[], byte[]>
    {
        public byte[] ConvertReceiveData(byte[] data, int length)
        {
            if (data.Length != length)
                return data.Take(length).ToArray();
            return data;
        }

        public byte[] ConvertSendData(byte[] obj)
        {
            return obj;
        }
    }
}
