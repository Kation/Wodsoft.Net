using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolConverter
    {
        private Dictionary<Type, object> _Converters;

        public ProtocolConverter()
        {
            _Converters = new Dictionary<Type, object>();
        }

        public T ConverterFrom<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            Type type = typeof(T);
            object item;
            if (!_Converters.TryGetValue(type, out item))
                throw new NotSupportedException("不支持的类型。");
            IValueConverter<T> converter = (IValueConverter<T>)item;
            return converter.ConverterFrom(stream);
        }

        public object ConverterFrom(Type type, Stream stream)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (stream == null)
                throw new ArgumentNullException("stream");
            object item;
            if (!_Converters.TryGetValue(type, out item))
                throw new NotSupportedException("不支持的类型。");
            dynamic converter = item;
            return converter.ConverterFrom(stream);
        }

        public byte[] ConverterTo<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Type type = typeof(T);
            object item;
            if (!_Converters.TryGetValue(type, out item))
                throw new NotSupportedException("不支持的类型。");
            IValueConverter<T> converter = (IValueConverter<T>)item;
            return converter.ConverterTo(value);
        }

        public byte[] ConverterTo(Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (value == null)
                throw new ArgumentNullException("value");
            object item;
            if (!_Converters.TryGetValue(type, out item))
                throw new NotSupportedException("不支持的类型。");
            dynamic converter = item;
            return converter.ConverterTo(value);
        }

        public void RegisterConverter<T>(IValueConverter<T> converter)
        {
            Type type = typeof(T);
            if (_Converters.ContainsKey(type))
                _Converters[type] = converter;
            else
                _Converters.Add(type, converter);
        }

        public void UnregisterConverter<T>(IValueConverter<T> converter)
        {
            Type type = typeof(T);
            _Converters.Remove(type);
        }

        public void RegisterDefaultConverter()
        {

        }
    }
}
