using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public class Package
    {
        public Package(int packageId,int commandId)
        {
            PackageId = packageId;
            CommandId = commandId;
        }

        public int PackageId { get; private set; }

        public int CommandId { get; private set; }

        public byte[] Data { get; set; }
    }
}
