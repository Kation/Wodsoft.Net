using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket流处理器上下文。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public class SocketStreamHandlerContext<TIn, TOut>
    {
        /// <summary>
        /// 初始化Socket流处理器上下文。
        /// </summary>
        /// <param name="stream">网络流。</param>
        public SocketStreamHandlerContext(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            Stream = stream;
            ReceiveContext = new SocketReceiveContext<TOut>(stream);
            SendContext = new SocketSendContext<TIn>(stream);
        }

        /// <summary>
        /// 获取Socket网络流。
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// 获取接收上下文。
        /// </summary>
        public SocketReceiveContext<TOut> ReceiveContext { get; private set; }

        /// <summary>
        /// 获取发送上下文。
        /// </summary>
        public SocketSendContext<TIn> SendContext { get; private set; }
    }
}
