using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public interface ISocket<TIn, TOut>
    {
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        void Send(TIn data);
        /// <summary>
        /// 异步发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        void SendAsync(TIn data);
        /// <summary>
        /// 断开连接。
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 异步断开连接。
        /// </summary>
        void DisconnectAsync();
        /// <summary>
        /// 断开完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs> DisconnectCompleted;
        /// <summary>
        /// 接收完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs<TOut>> ReceiveCompleted;
        /// <summary>
        /// 发送完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs<TIn>> SendCompleted;
        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }
        /// <summary>
        /// 获取本地终结点地址。
        /// </summary>
        IPEndPoint LocalEndPoint { get; }
    }
}
