using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolMappingResult : ProtocolMapping
    {
        public ProtocolMappingResult()
        {
            _Arguments = new List<Type>();
            Arguments = new ReadOnlyCollection<Type>(_Arguments);
        }

        private List<Type> _Arguments;
        public ReadOnlyCollection<Type> Arguments { get; private set; }

        public bool IsSealed { get; private set; }

        public Delegate InvokeMethod { get; private set; }

        private void CheckSeal()
        {
            if (IsSealed)
                throw new InvalidOperationException("映射结果已封装完毕。");
        }

        private void SetDelegate(Delegate invokeMethod)
        {
            InvokeMethod = invokeMethod;
            IsSealed = true;
        }
        
        public void Get(Action<ProtocolSession> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
        }

        public void Get<T1>(Action<ProtocolSession, T1> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
        }

        public void Get<T1, T2>(Action<ProtocolSession, T1, T2> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
        }

        public void Get<T1, T2, T3>(Action<ProtocolSession, T1, T2, T3> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
        }

        public void Get<T1, T2, T3, T4>(Action<ProtocolSession, T1, T2, T3, T4> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
        }

        public void Get<T1, T2, T3, T4, T5>(Action<ProtocolSession, T1, T2, T3, T4, T5> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
        }

        public void Get<T1, T2, T3, T4, T5, T6>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
            _Arguments.Add(typeof(T11));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
            _Arguments.Add(typeof(T11));
            _Arguments.Add(typeof(T12));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
            _Arguments.Add(typeof(T11));
            _Arguments.Add(typeof(T12));
            _Arguments.Add(typeof(T13));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
            _Arguments.Add(typeof(T11));
            _Arguments.Add(typeof(T12));
            _Arguments.Add(typeof(T13));
            _Arguments.Add(typeof(T14));
        }

        public void Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<ProtocolSession, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> invokeMethod)
        {
            if (invokeMethod == null)
                throw new ArgumentNullException("invokeMethod");
            CheckSeal();
            SetDelegate(invokeMethod);
            _Arguments.Add(typeof(T1));
            _Arguments.Add(typeof(T2));
            _Arguments.Add(typeof(T3));
            _Arguments.Add(typeof(T4));
            _Arguments.Add(typeof(T5));
            _Arguments.Add(typeof(T6));
            _Arguments.Add(typeof(T7));
            _Arguments.Add(typeof(T8));
            _Arguments.Add(typeof(T9));
            _Arguments.Add(typeof(T10));
            _Arguments.Add(typeof(T11));
            _Arguments.Add(typeof(T12));
            _Arguments.Add(typeof(T13));
            _Arguments.Add(typeof(T14));
            _Arguments.Add(typeof(T15));
        }

        public override Task Execute(ProtocolContext context)
        {
            if (!IsSealed)
                throw new NotSupportedException("映射结果未封装完毕。");
            object[] args = new object[_Arguments.Count + 1];
            args[0] = context.Session;
            for (int i = 0; i < _Arguments.Count; i++)
                args[i + 1] = context.Converter.ConverterFrom(_Arguments[i], context.Stream);
            return Task.Run(() =>
            {
                InvokeMethod.DynamicInvoke(args);
            });
        }
    }
}
