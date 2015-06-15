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
    public abstract class SocketUdpBase<TIn, TOut> : ISocket<TIn, TOut>
    {
        protected SocketUdpBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            Socket = socket;
            Handler = socketHandler;
        }

        protected Socket Socket { get; private set; }

        public ISocketHandler<TIn, TOut> Handler { get; private set; }

        protected SocketHandlerContext<TIn, TOut> HandlerContext { get; private set; }

        protected void Initialize()
        {
            HandlerContext = new SocketHandlerContext<TIn, TOut>(new SocketUdpStream(Socket, RemoteEndPoint));
        }

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
            if (success &&SendCompleted != null)
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

        public bool IsConnected { get; protected set; }

        public virtual void Disconnect()
        {
            throw new NotSupportedException("Udp不支持断开连接。");
        }

        public virtual Task DisconnectAsync()
        {
            throw new NotSupportedException("Udp不支持断开连接。");
        }

        public IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
        {
            throw new NotSupportedException("Udp不支持断开连接。");
        }

        public void EndDisconnect(IAsyncResult ar)
        {
            throw new NotSupportedException("Udp不支持断开连接。");
        }

        public IPEndPoint RemoteEndPoint { get; protected set; }

        public IPEndPoint LocalEndPoint { get; protected set; }
    }
}
