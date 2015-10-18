using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public abstract class ProtocolPackage
    {
        public abstract byte[] GetData();
    }
}
