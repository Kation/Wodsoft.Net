using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public abstract class ProtocolChannel
    {
        public ProtocolChannel()
        {
            Protocol = new ProtocolManager();
            Initialize();
        }

        protected ProtocolManager Protocol { get; private set; }

        private void Initialize()
        {
            MappingItem mapping = new MappingItem();

            foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (!method.IsFamily)
                    continue;
                var protocolAttribute = method.GetCustomAttribute<ProtocolAttribute>();
                if (protocolAttribute == null)
                    continue;
                var parameters = method.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(ProtocolSession))
                    continue;
                MappingItem parent = mapping;
                foreach (var arg in protocolAttribute.Arguments)
                {
                    MappingItem item = parent.Items.SingleOrDefault(t => t.Key == arg);
                    if (item == null)
                    {
                        item = new MappingItem();
                        item.Key = arg;
                        parent.Items.Add(item);
                    }
                    parent = item;
                }
                parent.Method = method;
            }

            if (mapping.Items.Count > 0)
            {
                if (mapping.Items.Select(t => t.Key.GetType()).Distinct().Count() != 1)
                    throw new NotSupportedException("处于同一级的协议标记类型不一致。");
                ProtocolMapping map = (ProtocolMapping)Activator.CreateInstance(typeof(ProtocolMapping<>).MakeGenericType(mapping.Items[0].Key.GetType()));
                Protocol.Map(map);
                RegisterMapping(map, mapping.Items);
            }
        }

        private void RegisterMapping(ProtocolMapping mapping, List<MappingItem> items)
        {
            foreach (var item in items)
            {
                if (item.Method == null)
                {
                    ProtocolMapping subMap = (ProtocolMapping)mapping.GetType().GetMethod("Map").MakeGenericMethod(item.Key.GetType()).Invoke(mapping, new object[] { item.Key });
                    RegisterMapping(subMap, item.Items);
                }
                else
                {
                    ProtocolMappingResult mapResult = (ProtocolMappingResult)mapping.GetType().GetMethod("On").Invoke(mapping, new object[] { item.Key });

                    List<ParameterExpression> parameters = new List<ParameterExpression>();
                    parameters.AddRange(item.Method.GetParameters().Select(t => Expression.Parameter(t.ParameterType)));
                    Expression call = Expression.Call(Expression.Constant(this), item.Method, parameters);
                    Delegate callDelegate = Expression.Lambda(call, parameters).Compile();

                    if (parameters.Count > 1)
                    {
                        var arguments = item.Method.GetParameters().Skip(1).Select(t => t.ParameterType).ToArray();
                        mapResult.GetType().GetMethods().Single(t => t.Name == "Get" && t.GetGenericArguments().Length == arguments.Length).MakeGenericMethod(arguments).Invoke(mapResult, new object[] { callDelegate });
                    }
                    else
                    {
                        mapResult.GetType().GetMethod("Get", new Type[] { typeof(Action<ProtocolSession>) }).Invoke(mapResult, new object[] { callDelegate });
                    }
                }
            }
        }

        private class MappingItem
        {
            public MappingItem()
            {
                Items = new List<MappingItem>();
            }

            public object Key { get; set; }

            public MethodInfo Method { get; set; }

            public List<MappingItem> Items { get; set; }
        }
    }
}
