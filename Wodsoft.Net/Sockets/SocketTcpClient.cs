using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket Tcp客户端。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public class SocketTcpClient<TIn, TOut> : SocketTcpBase<TIn, TOut>
    {
        /// <summary>
        /// 实例化SocketTcpClient。
        /// </summary>
        /// <param name="handler">Socket处理器。</param>
        public SocketTcpClient(ISocketStreamHandler<TIn, TOut> handler)
            : base(new Socket(SocketType.Stream, ProtocolType.Tcp), handler)
        { }

        /// <summary>
        /// 实例化SocketTcpClient。
        /// </summary>
        /// <param name="handler">Socket处理器。</param>
        /// <param name="streamProvider">Socket流提供器。</param>
        public SocketTcpClient(ISocketStreamHandler<TIn, TOut> handler, ISocketStreamProvider streamProvider)
            : base(new Socket(SocketType.Stream, ProtocolType.Tcp), handler, streamProvider)
        { }


        /// <summary>
        /// 使用已处理过的Socket实例化SocketTcpClient。
        /// </summary>
        /// <param name="socket">处理过的Socket。</param>
        /// <param name="handler">Socket处理器。</param>
        /// <param name="streamProvider">Socket流提供器。</param>
        public SocketTcpClient(Socket socket, ISocketStreamHandler<TIn, TOut> handler, ISocketStreamProvider streamProvider)
            : base(socket, handler, streamProvider)
        { }

        #region 连接

        /// <summary>
        /// 连接至服务器。
        /// </summary>
        /// <param name="endPoint">服务器终结点。</param>
        public bool Connect(IPEndPoint endPoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            try
            {
                Socket.Connect(endPoint);
                if (ConnectCompleted != null)
                    ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
                Initialize();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步连接至服务器。
        /// </summary>
        /// <param name="endPoint"></param>
        public async Task<bool> ConnectAsync(IPEndPoint endPoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            try
            {
                await Task.Factory.FromAsync(Socket.BeginConnect(endPoint, null, null), Socket.EndConnect);
                if (ConnectCompleted != null)
                    ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
                await InitializeAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 连接完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs> ConnectCompleted;
    }
}
