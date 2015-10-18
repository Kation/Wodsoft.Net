using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public interface IValueConverter<T>
    {
        T ConverterFrom(Stream stream);

        byte[] ConverterTo(T value);
    }
}
