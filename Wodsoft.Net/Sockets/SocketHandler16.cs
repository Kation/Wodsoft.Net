using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketHandler16 : SocketHeadContentHandler<byte[], byte[]>
    {
        public SocketHandler16() : base(2, 4096) { }
        
        protected override bool ProcessReceiveHead(SocketReceiveContext<byte[]> context)
        {
            if (context.Buffer.Length < 2)
                return false;
            byte[] data = new byte[2];
            context.Buffer.Read(data, 0, 2);
            context.DataBag.DataLength = BitConverter.ToUInt16(data, 0);
            return true;
        }

        protected override bool ProcessReceiveContent(SocketReceiveContext<byte[]> context)
        {
            ushort length = context.DataBag.DataLength;
            if (context.Buffer.Length < length)
                return false;
            byte[] data = new byte[length];
            context.Buffer.Read(data, 0, length);
            context.Result = data;
            return true;
        }

        protected override byte[] ProcessSendHead(SocketSendContext<byte[]> context)
        {
            return BitConverter.GetBytes((ushort)context.Data.Length);
        }

        protected override byte[] ProcessSendContent(SocketSendContext<byte[]> context)
        {
            return context.Data;
        }
    }
}
