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
            if (context.Buffer.Length < 16)
                return false;
            int index = context.Buffer.IndexOfValues(80, 0, 128, 255);
            if (index == -1)
            {
                context.Buffer.Clear();
                return false;
            }
            if (context.Buffer.Length < index + 16)
                return false;
            context.Buffer.Position = index + 4;
            byte[] data = new byte[4];
            context.Buffer.Read(data, 0, 4);
            int packageId = BitConverter.ToInt32(data, 0);
            context.Buffer.Read(data, 0, 4);
            int commandId = BitConverter.ToInt32(data, 0);
            context.Result = new Package(packageId, commandId);
            context.Buffer.Read(data, 0, 4);
            int packageSize = BitConverter.ToInt32(data, 0);
            context.Result.Data = new byte[packageSize];
            return true;
        }

        protected override bool ProcessReceiveContent(SocketReceiveContext<Package> context)
        {
            if (context.Buffer.Length < context.Result.Data.Length)
                return false;
            context.Buffer.Read(context.Result.Data, 0, context.Result.Data.Length);
            return true;
        }

        protected override byte[] ProcessSendHead(SocketSendContext<Package> context)
        {
            byte[] data = new byte[16];
            data[0] = 80;
            data[1] = 0;
            data[2] = 128;
            data[3] = 255;
            byte[] packageId = BitConverter.GetBytes(context.Data.PackageId);
            byte[] commandId = BitConverter.GetBytes(context.Data.CommandId);
            byte[] packageSize = BitConverter.GetBytes(context.Data.Data.Length);
            packageId.CopyTo(data, 4);
            commandId.CopyTo(data, 8);
            commandId.CopyTo(data, 12);
            return data;
        }

        protected override byte[] ProcessSendContent(SocketSendContext<Package> context)
        {
            return context.Data.Data;
        }
    }
}
