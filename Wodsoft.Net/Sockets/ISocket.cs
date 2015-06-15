using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public interface ISocket
    {
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 断开连接。
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 异步断开连接。
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// 异步开始断开连接。
        /// </summary>
        /// <param name="callback">异步回调。</param>
        /// <param name="state">异步状态对象。</param>
        /// <returns>异步结果。</returns>
        IAsyncResult BeginDisconnect(AsyncCallback callback, object state);

        /// <summary>
        /// 异步结束断开连接。
        /// </summary>
        /// <param name="ar">异步结果。</param>
        void EndDisconnect(IAsyncResult ar);

        /// <summary>
        /// 断开完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs> DisconnectCompleted;

        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取本地终结点地址。
        /// </summary>
        IPEndPoint LocalEndPoint { get; }
    }

    /// <summary>
    /// Socket支持接口。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public interface ISocket<TIn, TOut> :ISocket
    {
        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        /// <returns>发送是否成功，True为成功，False为失败。</returns>
        bool Send(TIn data);
        /// <summary>
        /// 异步发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        /// <returns>发送是否成功，True为成功，False为失败。</returns>
        Task<bool> SendAsync(TIn data);
        /// <summary>
        /// 异步开始发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        /// <param name="callback">异步回调。</param>
        /// <param name="state">异步状态对象。</param>
        /// <returns>异步结果。</returns>
        IAsyncResult BeginSend(TIn data, AsyncCallback callback, object state);
        /// <summary>
        /// 异步结束发送数据。
        /// </summary>
        /// <param name="ar">异步结果。</param>
        /// <returns>发送是否成功，True为成功，False为失败。</returns>
        bool EndSend(IAsyncResult ar);
        /// <summary>
        /// 接收数据。
        /// </summary>
        /// <returns>收到的数据。</returns>
        TOut Receive();
        /// <summary>
        /// 异步接收数据。
        /// </summary>
        /// <returns>接收数据任务。</returns>
        Task<TOut> ReceiveAsync();
        /// <summary>
        /// 异步开始接收数据。
        /// </summary>
        /// <param name="callback">异步回调。</param>
        /// <param name="state">异步状态对象。</param>
        /// <returns>异步结果。</returns>
        IAsyncResult BeginReceive(AsyncCallback callback, object state);
        /// <summary>
        /// 异步结束接收数据。
        /// </summary>
        /// <param name="ar">异步结果。</param>
        /// <returns>收到的数据。</returns>
        TOut EndReceive(IAsyncResult ar);
        /// <summary>
        /// 接收完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs<TOut>> ReceiveCompleted;
        /// <summary>
        /// 发送完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs<TIn>> SendCompleted;
    }
}
