using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket处理器接口。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public interface ISocketHandler<TIn, TOut>
    {
        /// <summary>
        /// 异步开始接收
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <param name="callback">回调函数</param>
        /// <param name="state">自定义状态</param>
        /// <returns>异步结果</returns>
        IAsyncResult BeginReceive(SocketHandlerContext<TIn,TOut> context, AsyncCallback callback, object state);
        /// <summary>
        /// 异步结束接收
        /// </summary>
        /// <param name="asyncResult">异步结果</param>
        /// <returns>接收到的数据</returns>
        TOut EndReceive(IAsyncResult asyncResult);
        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns>收到的数据</returns>
        TOut Receive(SocketHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 异步接收
        /// </summary>
        /// <param name="context">处理上下文</param>
        /// <returns>任务结果</returns>
        Task<TOut> ReceiveAsync(SocketHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="context">处理上下文</param>
        /// <param name="callback">回调函数</param>
        /// <param name="state">自定义状态</param>
        /// <returns>异步结果</returns>
        IAsyncResult BeginSend(TIn data, SocketHandlerContext<TIn, TOut> context, AsyncCallback callback, object state);
        /// <summary>
        /// 结束发送
        /// </summary>
        /// <param name="asyncResult">异步结果</param>
        /// <returns>发送是否成功</returns>
        bool EndSend(IAsyncResult asyncResult);
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="context">处理上下文</param>
        /// <returns>是否成功</returns>
        bool Send(TIn data, SocketHandlerContext<TIn, TOut> context);
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="context">处理上下文</param>
        /// <returns>任务结果</returns>
        Task<bool> SendAsync(TIn data, SocketHandlerContext<TIn, TOut> context);
    }
}
