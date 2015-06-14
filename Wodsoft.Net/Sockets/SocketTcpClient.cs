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
    public class SocketTcpClient<TIn, TOut> : SocketBase<TIn, TOut>
    {
        /// <summary>
        /// 实例化SocketTcpClient。
        /// </summary>
        /// <param name="handler">Socket处理器。</param>
        /// <param name="ipv6">是否使用IPv6协议，否则为IPv4。</param>
        public SocketTcpClient(ISocketHandler<TIn, TOut> handler, bool ipv6 = false)
            : base(new Socket(SocketType.Stream, ipv6 ? ProtocolType.IPv6 : ProtocolType.IPv4), handler)
        {

        }

        /// <summary>
        /// 使用已处理过的Socket实例化SocketTcpClient。
        /// </summary>
        /// <param name="socket">处理过的Socket。</param>
        /// <param name="handler">Socket处理器。</param>
        public SocketTcpClient(Socket socket, ISocketHandler<TIn, TOut> handler)
            : base(socket, handler)
        {
            Initialize();
        }
        
        #region 连接

        /// <summary>
        /// 连接至服务器。
        /// </summary>
        /// <param name="endpoint">服务器终结点。</param>
        public void Connect(IPEndPoint endpoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            //锁定自己，避免多线程同时操作
            lock (this)
            {
                SocketAsyncState<TOut> state = new SocketAsyncState<TOut>();
                //Socket异步连接
                Socket.BeginConnect(endpoint, EndConnect, state).AsyncWaitHandle.WaitOne();
                //等待异步全部处理完成
                while (!state.Completed)
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// 异步连接至服务器。
        /// </summary>
        /// <param name="endpoint"></param>
        public void ConnectAsync(IPEndPoint endpoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            //锁定自己，避免多线程同时操作
            lock (this)
            {
                SocketAsyncState<TOut> state = new SocketAsyncState<TOut>();
                //设置状态为异步
                state.IsAsync = true;
                //Socket异步连接
                Socket.BeginConnect(endpoint, EndConnect, state);
            }
        }

        private void EndConnect(IAsyncResult result)
        {
            SocketAsyncState<TOut> state = (SocketAsyncState<TOut>)result.AsyncState;

            try
            {
                Socket.EndConnect(result);
            }
            catch
            {
                //出现异常，连接失败。
                state.Completed = true;
                //判断是否为异步，异步则引发事件
                if (state.IsAsync && ConnectCompleted != null)
                    ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
                return;
            }
            Initialize();
            //连接完成
            state.Completed = true;
            if (state.IsAsync && ConnectCompleted != null)
            {
                ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
            }
        }

        #endregion

        /// <summary>
        /// 连接完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs> ConnectCompleted;
    }
}
