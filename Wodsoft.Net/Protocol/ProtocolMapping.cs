using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public abstract class ProtocolMapping
    {
        protected ProtocolMapping() { }

        public abstract Task Execute(ProtocolContext context);
    }

    public class ProtocolMapping<TKey> : ProtocolMapping
        where TKey : struct
    {
        private Dictionary<TKey, ProtocolMapping> _Mappings;
        public ProtocolMapping()
        {
            _Mappings = new Dictionary<TKey, ProtocolMapping>();
        }

        public ProtocolMapping<T> Map<T>(TKey key)
            where T : struct
        {
            if (_Mappings.ContainsKey(key))
                throw new ArgumentException("Key已存在。", "key");
            ProtocolMapping<T> mapping = new ProtocolMapping<T>();
            _Mappings.Add(key, mapping);
            return mapping;
        }

        public ProtocolMappingResult On(TKey key)
        {
            if (_Mappings.ContainsKey(key))
                throw new ArgumentException("Key已存在。", "key");
            ProtocolMappingResult mapping = new ProtocolMappingResult();
            _Mappings.Add(key, mapping);
            return mapping;
        }

        public override async Task Execute(ProtocolContext context)
        {
            TKey key = context.Converter.ConverterFrom<TKey>(context.Stream);
            ProtocolMapping mapping;
            if (_Mappings.TryGetValue(key, out mapping))
                await mapping.Execute(context);
        }
    }
}
