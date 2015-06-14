using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// 实例化Socket事件参数
        /// </summary>
        /// <param name="operation">操作类型</param>
        public SocketEventArgs(SocketAsyncOperation operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// 获取事件操作类型。
        /// </summary>
        public SocketAsyncOperation Operation { get; private set; }
    }

    /// <summary>
    /// Socket事件参数
    /// </summary>
    public class SocketEventArgs<T> : SocketEventArgs
    {
        /// <summary>
        /// 实例化Socket事件参数
        /// </summary>
        /// <param name="operation">操作类型</param>
        public SocketEventArgs(T data, SocketAsyncOperation operation)
            : base(operation)
        {
            Data = data;
        }

        /// <summary>
        /// 获取或设置事件相关数据。
        /// </summary>
        public T Data { get; private set; }
    }
}
