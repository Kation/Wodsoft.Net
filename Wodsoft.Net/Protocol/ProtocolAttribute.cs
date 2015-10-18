using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolAttribute : Attribute
    {
        public ProtocolAttribute(params object[] args)
        {
            Arguments = args;
        }

        public object[] Arguments { get; private set; }
    }
}
