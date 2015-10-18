using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketDataBag : DynamicObject, IDictionary<string, object>
    {
        private Dictionary<string, object> _Dictionary;

        public SocketDataBag()
        {
            _Dictionary = new Dictionary<string, object>();
        }

        #region IDictionary

        public void Add(string key, object value)
        {
            _Dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _Dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys { get { return _Dictionary.Keys; } }

        public bool Remove(string key) { return _Dictionary.Remove(key); }

        public bool TryGetValue(string key, out object value)
        {
            return _Dictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values { get { return _Dictionary.Values; } }

        public object this[string key]
        {
            get
            {
                object value;
                _Dictionary.TryGetValue(key, out value);
                return value;
            }
            set
            {
                if (_Dictionary.ContainsKey(key))
                    _Dictionary[key] = value;
                else
                    _Dictionary.Add(key, value);
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((IDictionary<string, object>)(_Dictionary)).Add(item);
        }

        public void Clear()
        {
            _Dictionary.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)(_Dictionary)).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)(_Dictionary)).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Dictionary.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)(_Dictionary)).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _Dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Dictionary.GetEnumerator();
        }

        #endregion

        #region Dynamic

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_Dictionary.TryGetValue(binder.Name, out result) && binder.ReturnType.IsValueType)
            {
                result = Activator.CreateInstance(binder.ReturnType);
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (binder.Name != "Clear")
                return false;
            if (args.Length != 0)
                return false;
            Clear();
            return true;
        }

        #endregion
    }
}
