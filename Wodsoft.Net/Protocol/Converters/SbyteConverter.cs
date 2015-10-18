using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol.Converters
{
    public class SbyteConverter : IValueConverter<sbyte>
    {
        public sbyte ConverterFrom(Stream stream)
        {
            return (sbyte)(byte)stream.ReadByte();
        }

        public byte[] ConverterTo(sbyte value)
        {
            return new byte[] { (byte)value };
        }
    }
}
