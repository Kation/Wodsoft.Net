using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolContext
    {
        public ProtocolContext(ProtocolSession session, Stream stream, ProtocolConverter converter)
        {
            Session = session;
            Stream = stream;
            Converter = converter;
        }

        public ProtocolConverter Converter { get; private set; }

        public Stream Stream { get; private set; }

        public ProtocolSession Session { get; private set; }
    }
}
