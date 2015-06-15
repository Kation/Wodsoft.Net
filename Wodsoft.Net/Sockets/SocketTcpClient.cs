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
        public SocketTcpClient(ISocketHandler<TIn, TOut> handler)
            : base(new Socket(SocketType.Stream, ProtocolType.Tcp), handler)
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

        public IAsyncResult BeginConnect(IPEndPoint endPoint, AsyncCallback callback, object state)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            SocketAsyncResult<bool> asyncResult = new SocketAsyncResult<bool>(state);
            SocketAsyncState asyncState = new SocketAsyncState();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            try
            {
                Socket.BeginConnect(endPoint, EndBeginConnect, asyncState);
            }
            catch
            {
                asyncResult.CompletedSynchronously = true;
                asyncResult.IsCompleted = true;
                asyncResult.Data = false;
                ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
                if (callback != null)
                    callback(asyncResult);
            }
            return asyncResult;
        }

        public bool EndConnect(IAsyncResult ar)
        {
            SocketAsyncResult<bool> asyncResult = ar as SocketAsyncResult<bool>;
            if (asyncResult == null)
                throw new ArgumentNullException("异步结果不属于该Socket。");
            return asyncResult.Data;
        }

        private void EndBeginConnect(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            SocketAsyncResult<bool> asyncResult = (SocketAsyncResult<bool>)state.AsyncResult;
            try
            {
                Socket.EndConnect(ar);
                BeginInitialize(EndBeginInitialize, state);

            }
            catch
            {
                //出现异常，连接失败。
                asyncResult.Data = false;
                asyncResult.IsCompleted = true;
                ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
                if (state.AsyncCallback != null)
                    state.AsyncCallback(asyncResult);
            }
        }

        private void EndBeginInitialize(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            SocketAsyncResult<bool> asyncResult = (SocketAsyncResult<bool>)state.AsyncResult;
            asyncResult.IsCompleted = true;
            asyncResult.Data = true;
            if (ConnectCompleted != null)
                ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            if (state.AsyncCallback != null)
                state.AsyncCallback(asyncResult);
        }

        #endregion

        /// <summary>
        /// 连接完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs> ConnectCompleted;
    }
}
