using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket异步状态。
    /// </summary>
    public class SocketAsyncState
    {
        /// <summary>
        /// 获取或设置异步回调方法。
        /// </summary>
        public AsyncCallback AsyncCallback { get; set; }

        /// <summary>
        /// 获取或设置异步回调结果。
        /// </summary>
        public IAsyncResult AsyncResult { get; set; }
    }

    /// <summary>
    /// Socket异步状态。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    public class SocketAsyncState<T> : SocketAsyncState
    {
        /// <summary>
        /// 获取或设置相关数据。
        /// </summary>
        public T Data { get; set; }
    }
}
