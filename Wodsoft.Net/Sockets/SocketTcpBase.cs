using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket Tcp基类。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public abstract class SocketTcpBase<TIn, TOut> : SocketBase<TIn, TOut>, IDisposable
    {
        /// <summary>
        /// 实例化TCP客户端。
        /// </summary>
        /// <param name="socket">Socket套接字。</param>
        /// <param name="socketHandler">Socket处理器。</param>
        protected SocketTcpBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler)
            : this(socket, socketHandler, new SocketNetworkStreamProvider()) { }

        /// <summary>
        /// 实例化TCP客户端。
        /// </summary>
        /// <param name="socket">Socket套接字。</param>
        /// <param name="socketHandler">Socket处理器。</param>
        /// <param name="streamProvider">Socket网络流提供者。</param>
        protected SocketTcpBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler, ISocketStreamProvider streamProvider)
            : base(socket, socketHandler, streamProvider)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            if (streamProvider == null)
                throw new ArgumentNullException("streamProvider");
            socket.NoDelay = true;
        }

        /// <summary>
        /// 获取Socket是否已连接。
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return Socket.Connected;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        #region 断开连接

        /// <summary>
        /// 断开与服务器的连接。
        /// </summary>
        public override void Disconnect()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            Socket.Disconnect(false);
            Disconnected(true);
        }

        /// <summary>
        /// 异步断开与服务器的连接。
        /// </summary>
        public override async Task DisconnectAsync()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            await Task.Factory.FromAsync(Socket.BeginDisconnect(true, null, null), Socket.EndDisconnect);
            Disconnected(true);
        }

        public override IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            SocketAsyncResult asyncResult = new SocketAsyncResult(state);
            SocketAsyncState asyncState = new SocketAsyncState();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            Socket.BeginDisconnect(true, EndBeginDisconnect, asyncState);
            return asyncResult;
        }

        public override void EndDisconnect(IAsyncResult ar)
        {

        }

        private void EndBeginDisconnect(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            SocketAsyncResult asyncResult = (SocketAsyncResult)state.AsyncResult;
            asyncResult.CompletedSynchronously = ar.CompletedSynchronously;
            asyncResult.IsCompleted = true;
            Socket.EndDisconnect(ar);
            Disconnected(true);
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            if (state.AsyncCallback != null)
                state.AsyncCallback(asyncResult);
        }

        #endregion
        
        #region 其它

        /// <summary>
        /// 释放资源。
        /// </summary>
        protected override void Disposed()
        {
            if (IsConnected)
                Socket.Disconnect(false);
            Socket.Close();
        }

        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        public sealed override IPEndPoint RemoteEndPoint
        {
            get
            {
                if (IsConnected)
                    return (IPEndPoint)Socket.RemoteEndPoint;
                return null;
            }
        }

        /// <summary>
        /// 获取本地终结点地址。
        /// </summary>
        public sealed override IPEndPoint LocalEndPoint
        {
            get
            {
                if (IsConnected)
                    return (IPEndPoint)Socket.LocalEndPoint;
                return null;
            }
        }

        #endregion
    }
}
