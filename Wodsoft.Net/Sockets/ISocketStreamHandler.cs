using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket流处理器接口。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public interface ISocketStreamHandler<TIn, TOut>
    {
        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns>收到的数据</returns>
        TOut Receive(SocketStreamHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 异步接收
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns>任务结果</returns>
        Task<TOut> ReceiveAsync(SocketStreamHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="context">处理上下文</param>
        /// <returns>是否成功</returns>
        bool Send(TIn data, SocketStreamHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="context">处理上下文</param>
        /// <returns>任务结果</returns>
        Task<bool> SendAsync(TIn data, SocketStreamHandlerContext<TIn, TOut> context);
    }
}
