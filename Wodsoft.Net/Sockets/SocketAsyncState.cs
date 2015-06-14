using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Wodsoft.Net.Sockets
{
    internal class SocketAsyncState<T>
    {
        /// <summary>
        /// 是否完成。
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// 是否异步
        /// </summary>
        public bool IsAsync { get; set; }
    }
}
