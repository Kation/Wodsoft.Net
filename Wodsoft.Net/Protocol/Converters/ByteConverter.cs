using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol.Converters
{
    public class ByteConverter : IValueConverter<byte>
    {
        public byte ConverterFrom(Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public byte[] ConverterTo(byte value)
        {
            return new byte[] { value };
        }
    }
}
