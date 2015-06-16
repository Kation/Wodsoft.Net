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
    /// <summary>
    /// Socket基类。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public abstract class SocketBase<TIn, TOut> : ISocket<TIn, TOut>, IDisposable
    {
        private SocketDataBag _DataBag;

        /// <summary>
        /// 实例化Socket基类。
        /// </summary>
        /// <param name="socket">Socket套接字。</param>
        /// <param name="socketHandler">Socket处理者。</param>
        /// <param name="streamProvider">Socket网络流提供者。</param>
        protected SocketBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler, ISocketStreamProvider streamProvider)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            if (streamProvider == null)
                throw new ArgumentNullException("streamProvider");
            Socket = socket;
            Handler = socketHandler;
            StreamProvider = streamProvider;
        }
        
        /// <summary>
        /// 获取Socket套接字。
        /// </summary>
        protected Socket Socket { get; private set; }

        /// <summary>
        /// 获取Socket处理者。
        /// </summary>
        public ISocketHandler<TIn, TOut> Handler { get; private set; }

        /// <summary>
        /// 获取Socket处理上下文。
        /// </summary>
        protected SocketHandlerContext<TIn, TOut> HandlerContext { get; private set; }

        /// <summary>
        /// 获取Socket网络流提供者。
        /// </summary>
        public ISocketStreamProvider StreamProvider { get; private set; }

        /// <summary>
        /// 获取或设置Socket是否已连接。
        /// </summary>
        public virtual bool IsConnected { get; protected set; }

        #region 初始化

        /// <summary>
        /// 初始化Socket连接。调用于Socket连接建立后。
        /// </summary>
        protected virtual void Initialize()
        {
            HandlerContext = new SocketHandlerContext<TIn, TOut>(StreamProvider.GetStream(Socket));
        }

        protected virtual async Task InitializeAsync()
        {
            HandlerContext = new SocketHandlerContext<TIn, TOut>(await StreamProvider.GetStreamAsync(Socket));
        }

        protected virtual IAsyncResult BeginInitialize(AsyncCallback callback, object state)
        {
            SocketAsyncResult asyncResult = new SocketAsyncResult(state);
            SocketAsyncState asyncState = new SocketAsyncState();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            StreamProvider.BeginGetStream(Socket, EndGetStream, asyncState);
            return asyncResult;
        }

        private void EndGetStream(IAsyncResult ar)
        {
            SocketAsyncState asyncState = (SocketAsyncState)ar.AsyncState;
            SocketAsyncResult asyncResult = (SocketAsyncResult)asyncState.AsyncResult;
            HandlerContext = new SocketHandlerContext<TIn, TOut>(StreamProvider.EndGetStream(ar));
            asyncResult.IsCompleted = true;
            asyncResult.CompletedSynchronously = true;
            if (asyncState.AsyncCallback != null)
                asyncState.AsyncCallback(asyncResult);
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
        }

        protected virtual void EndInitialize(IAsyncResult ar)
        {

        }

        #endregion

        #region 断开连接

        /// <summary>
        /// 断开与服务器的连接。
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 异步断开与服务器的连接。
        /// </summary>
        public abstract Task DisconnectAsync();

        public abstract IAsyncResult BeginDisconnect(AsyncCallback callback, object state);

        public abstract void EndDisconnect(IAsyncResult ar);

        /// <summary>
        /// 断开连接后调用的方法。
        /// </summary>
        /// <param name="raiseEvent">是否触发事件。</param>
        protected void Disconnected(bool raiseEvent)
        {
            HandlerContext = null;
            if (raiseEvent && DisconnectCompleted != null)
                DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
        }

        #endregion

        #region 发送数据

        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        public bool Send(TIn data)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            //发送的数据不能为null
            if (data == null)
                throw new ArgumentNullException("data");

            //开始发送数据
            if (!Handler.Send(data, HandlerContext))
            {
                Disconnected(true);
                return false;
            }
            else
            {
                if (SendCompleted != null)
                    SendCompleted(this, new SocketEventArgs<TIn>(data, SocketAsyncOperation.Send));
                return true;
            }
        }

        /// <summary>
        /// 异步发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        public async Task<bool> SendAsync(TIn data)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            //发送的数据不能为null
            if (data == null)
                throw new ArgumentNullException("data");

            if (!await Handler.SendAsync(data, HandlerContext))
            {
                Disconnected(true);
                return false;
            }
            else
            {
                if (SendCompleted != null)
                    SendCompleted(this, new SocketEventArgs<TIn>(data, SocketAsyncOperation.Send));
                return true;
            }
        }

        public IAsyncResult BeginSend(TIn data, AsyncCallback callback, object state)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            //发送的数据不能为null
            if (data == null)
                throw new ArgumentNullException("data");

            SocketAsyncResult<bool> asyncResult = new SocketAsyncResult<bool>(state);
            SocketAsyncState<TIn> asyncState = new SocketAsyncState<TIn>();
            asyncState.AsyncCallback = callback;
            asyncState.AsyncResult = asyncResult;
            asyncState.Data = data;
            Handler.BeginSend(data, HandlerContext, EndBeginSend, asyncState);
            return asyncResult;
        }

        public bool EndSend(IAsyncResult ar)
        {
            SocketAsyncResult<bool> asyncResult = ar as SocketAsyncResult<bool>;
            if (asyncResult == null)
                throw new ArgumentException("异步结果不属于该Socket。");
            return asyncResult.Data;
        }

        private void EndBeginSend(IAsyncResult ar)
        {
            SocketAsyncState<TIn> state = (SocketAsyncState<TIn>)ar.AsyncState;
            SocketAsyncResult<bool> asyncResult = (SocketAsyncResult<bool>)state.AsyncResult;
            bool success = Handler.EndSend(ar);
            asyncResult.Data = success;
            asyncResult.CompletedSynchronously = ar.CompletedSynchronously;
            asyncResult.IsCompleted = true;
            if (success && SendCompleted != null)
                SendCompleted(this, new SocketEventArgs<TIn>(state.Data, SocketAsyncOperation.Send));
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            if (state.AsyncCallback != null)
                state.AsyncCallback(state.AsyncResult);
        }

        #endregion

        #region 接收数据

        public TOut Receive()
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            TOut value = Handler.Receive(HandlerContext);
            if (value == null)
            {
                Disconnect();
            }
            else
                if (ReceiveCompleted != null)
                    ReceiveCompleted(this, new SocketEventArgs<TOut>(value, SocketAsyncOperation.Receive));
            return value;
        }

        public async Task<TOut> ReceiveAsync()
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            TOut value = await Handler.ReceiveAsync(HandlerContext);
            if (value == null)
            {
                await DisconnectAsync();
            }
            else
                if (ReceiveCompleted != null)
                    ReceiveCompleted(this, new SocketEventArgs<TOut>(value, SocketAsyncOperation.Receive));
            return value;
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);

            SocketAsyncResult<TOut> ar = new SocketAsyncResult<TOut>(state);
            SocketAsyncState asyncState = new SocketAsyncState();
            asyncState.AsyncCallback = callback;
            Handler.BeginReceive(HandlerContext, EndBeginReceive, asyncState);
            return ar;
        }

        public TOut EndReceive(IAsyncResult ar)
        {
            SocketAsyncResult<TOut> asyncResult = ar as SocketAsyncResult<TOut>;
            if (asyncResult == null)
                throw new ArgumentException("异步结果不属于该Socket。");
            return asyncResult.Data;
        }

        private void EndBeginReceive(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            SocketAsyncResult<TOut> asyncResult = (SocketAsyncResult<TOut>)state.AsyncResult;
            TOut data = Handler.EndReceive(ar);
            if (data == null)
            {
                asyncResult.IsCompleted = false;
                Task.Run((Func<Task>)DisconnectAsync);
            }
            else
                asyncResult.IsCompleted = true;
            asyncResult.CompletedSynchronously = ar.CompletedSynchronously;
            asyncResult.Data = data;
            if (ReceiveCompleted != null)
                ReceiveCompleted(this, new SocketEventArgs<TOut>(data, SocketAsyncOperation.Receive));
            ((AutoResetEvent)asyncResult.AsyncWaitHandle).Set();
            if (state.AsyncCallback != null)
                state.AsyncCallback(asyncResult);
        }

        public async Task ReceiveCycle()
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            while (IsConnected)
            {
                TOut data = await Handler.ReceiveAsync(HandlerContext);
                if (data == null)
                {
                    await DisconnectAsync();
                    return;
                }
                if (ReceiveCompleted != null)
                    ReceiveCompleted(this, new SocketEventArgs<TOut>(data, SocketAsyncOperation.Receive));
            }
        }

        #endregion

        #region 事件

        ///// <summary>
        ///// 断开完成时引发事件。
        ///// </summary>
        public event EventHandler<SocketEventArgs> DisconnectCompleted;
        ///// <summary>
        ///// 接收完成时引发事件。
        ///// </summary>
        public event EventHandler<SocketEventArgs<TOut>> ReceiveCompleted;
        ///// <summary>
        ///// 发送完成时引发事件。
        ///// </summary>
        public event EventHandler<SocketEventArgs<TIn>> SendCompleted;

        #endregion

        #region 字典

        public dynamic DataBag
        {
            get
            {
                if (_DataBag == null)
                    _DataBag = new SocketDataBag();
                return _DataBag;
            }
        }

        #endregion

        #region 其它

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Disposed();
            if (_DataBag != null)
            {
                _DataBag.Clear();
                _DataBag = null;
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        protected virtual void Disposed()
        {

        }

        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取本地终结点地址。
        /// </summary>
        public abstract IPEndPoint LocalEndPoint { get; }

        #endregion
    }
}
