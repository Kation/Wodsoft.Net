using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpClient<TIn, TOut> : SocketUdpBase<TIn, TOut>
    {
        public SocketUdpClient(ISocketHandler<TIn, TOut> socketHandler) :
            base(new Socket(SocketType.Dgram, ProtocolType.Udp), socketHandler)
        {

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
                _RemoteEndPoint = endPoint;
                IsConnected = true;
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
                _RemoteEndPoint = endPoint;
                IsConnected = true;
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
            SocketAsyncState<IPEndPoint> asyncState = new SocketAsyncState<IPEndPoint>();
            asyncState.Data = endPoint;
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
            SocketAsyncState<IPEndPoint> state = (SocketAsyncState<IPEndPoint>)ar.AsyncState;
            SocketAsyncResult<bool> asyncResult = (SocketAsyncResult<bool>)state.AsyncResult;
            try
            {
                Socket.EndConnect(ar);
                _RemoteEndPoint = state.Data;
                IsConnected = true;
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

        #region 断开

        public override void Disconnect()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            _RemoteEndPoint = null;
            IsConnected = false;
            Disconnected(true);
        }

        public override Task DisconnectAsync()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            return Task.Run(() =>
            {
                _RemoteEndPoint = null;
                IsConnected = false;
                Disconnected(true);
            });
        }

        public override IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            _RemoteEndPoint = null;
            IsConnected = false;
            Disconnected(true);
            SocketAsyncResult asyncResult = new SocketAsyncResult(state);
            asyncResult.IsCompleted = true;
            asyncResult.CompletedSynchronously = true;
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            return asyncResult;
        }

        public override void EndDisconnect(IAsyncResult ar) { }

        #endregion

        /// <summary>
        /// 连接完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs> ConnectCompleted;

        private IPEndPoint _RemoteEndPoint;
        public override IPEndPoint RemoteEndPoint
        {
            get { return _RemoteEndPoint; }
        }
    }
}
