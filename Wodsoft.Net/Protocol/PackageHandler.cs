using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Protocol
{
    public class PackageHandler : SocketHeadContentHandler<Package, Package>
    {
        public PackageHandler() : base(8, 4096) { }

        protected override bool ProcessReceiveHead(SocketReceiveContext<Package> context)
        {
            if (context.Buffer.Length < 12)
                return false;
            if (context.Buffer.IndexOfValues(80,0,128,255) == -1)
            {
                context.Buffer.Clear();
                return false;
            }
            byte[] data = new byte[4];
            context.Buffer.Read(data, 0, 4);
            int packageId = BitConverter.ToInt32(data, 0);
            context.Buffer.Read(data, 0, 4);
            int commandId = BitConverter.ToInt32(data, 0);
            context.Result = new Package(packageId, commandId);
            return true;
        }

        protected override bool ProcessReceiveContent(SocketReceiveContext<Package> context)
        {
            throw new NotImplementedException();
        }

        protected override byte[] ProcessSendHead(SocketSendContext<Package> context)
        {
            byte[] data = new byte[12];
            data[0] = 80;
            data[1] = 0;
            data[2] = 128;
            data[3] = 255;
            byte[] packageId = BitConverter.GetBytes(context.Data.PackageId);
            byte[] commandId = BitConverter.GetBytes(context.Data.CommandId);
            packageId.CopyTo(data, 4);
            commandId.CopyTo(data, 8);
            return data;
        }

        protected override byte[] ProcessSendContent(SocketSendContext<Package> context)
        {
            throw new NotImplementedException();
        }
    }
}
