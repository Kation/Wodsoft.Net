using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketHandlerAsyncResult<TIn, TOut> : SocketAsyncResult
    {
        /// <summary>
        /// 实例化Socket异步操作状态
        /// </summary>
        /// <param name="state"></param>
        public SocketHandlerAsyncResult(SocketHandlerContext<TIn, TOut> context, object state)
            : base(state)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
        }

        /// <summary>
        /// 获取处理上下文。
        /// </summary>
        public SocketHandlerContext<TIn, TOut> Context { get; private set; }
    }
}
