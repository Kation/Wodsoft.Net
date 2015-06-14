using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Wodsoft.Net.Sockets
{
    public abstract class SocketBase<TIn, TOut> : ISocket<TIn, TOut>, IDisposable
    {
        protected SocketHandlerContext<TIn, TOut> HandlerContext { get; private set; }

        protected Socket Socket { get; private set; }

        /// <summary>
        /// 实例化TCP客户端。
        /// </summary>
        protected SocketBase(Socket socket, ISocketHandler<TIn, TOut> socketHandler)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            Socket = socket;
            socket.NoDelay = true;
            Handler = socketHandler;
            //HandlerContext = new SocketHandlerContext<TIn, TOut>();
        }

        /// <summary>
        /// Socket处理程序
        /// </summary>
        public ISocketHandler<TIn, TOut> Handler { get; private set; }

        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        public bool IsConnected { get { return Socket.Connected; } }

        protected void Initialize()
        {
            HandlerContext = new SocketHandlerContext<TIn, TOut>(GetNetworkStream());

            SocketAsyncState<TOut> state = new SocketAsyncState<TOut>();
            //开始接收数据
            Handler.BeginReceive(HandlerContext, EndReceive, state);
        }

        protected virtual Stream GetNetworkStream()
        {
            return new NetworkStream(Socket);
        }

        #region 断开连接

        /// <summary>
        /// 断开与服务器的连接。
        /// </summary>
        public virtual void Disconnect()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            lock (this)
            {
                //Socket异步断开并等待完成
                try
                {
                    Socket.BeginDisconnect(true, EndDisconnect, true).AsyncWaitHandle.WaitOne();
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 异步断开与服务器的连接。
        /// </summary>
        public virtual void DisconnectAsync()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            lock (this)
            {
                //Socket异步断开
                Socket.BeginDisconnect(true, EndDisconnect, false);
            }
        }

        private void EndDisconnect(IAsyncResult result)
        {
            try
            {
                Socket.EndDisconnect(result);
            }
            catch
            {

            }
            //是否同步
            bool sync = (bool)result.AsyncState;

            if (!sync && DisconnectCompleted != null)
            {
                DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
            }
        }

        //这是一个给收发异常准备的断开引发事件方法
        private void Disconnected(bool raiseEvent)
        {
            if (raiseEvent && DisconnectCompleted != null)
                DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
        }

        #endregion

        #region 发送数据

        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        public void Send(TIn data)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            //发送的数据不能为null
            if (data == null)
                throw new ArgumentNullException("data");

            //开始发送数据
            if (!Handler.Send(data, HandlerContext))
                Disconnected(true);
        }

        /// <summary>
        /// 异步发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        public void SendAsync(TIn data)
        {
            //是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            //发送的数据不能为null
            if (data == null)
                throw new ArgumentNullException("data");

            //设置异步状态
            SocketAsyncState<TIn> state = new SocketAsyncState<TIn>();
            state.IsAsync = true;
            state.Data = data;
            try
            {
                //开始发送数据并等待完成
                Handler.BeginSend(data, HandlerContext, EndSend, state);
            }
            catch
            {
                //出现异常则断开Socket连接
                Disconnected(true);
            }
        }

        private void EndSend(IAsyncResult result)
        {
            SocketAsyncState<TIn> state = (SocketAsyncState<TIn>)result.AsyncState;

            //是否完成
            state.Completed = Handler.EndSend(result);
            //没有完成则断开Socket连接
            if (!state.Completed)
                Disconnected(true);

            //引发发送结束事件
            if (state.IsAsync && SendCompleted != null)
            {
                SendCompleted(this, new SocketEventArgs<TIn>(state.Data, SocketAsyncOperation.Send));
            }
        }

        #endregion

        #region 接收数据



        private void EndReceive(IAsyncResult result)
        {
            SocketAsyncState<TOut> state = (SocketAsyncState<TOut>)result.AsyncState;
            //接收到的数据
            TOut data = Handler.EndReceive(result);
            ////如果数据长度为0，则断开Socket连接
            //if (data.Length == 0)
            //{
            //    Disconnected(true);
            //    return;
            //}
            //再次开始接收数据
            Handler.BeginReceive(HandlerContext, EndReceive, state);

            //引发接收完成事件
            if (ReceiveCompleted != null)
                ReceiveCompleted(this, new SocketEventArgs<TOut>(data, SocketAsyncOperation.Receive));
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

        private Dictionary<string, object> data;
        /// <summary>
        /// 获取或设置字典。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                key = key.ToLower();
                if (data.ContainsKey(key))
                    return data[key];
                return null;
            }
            set
            {

                key = key.ToLower();
                if (value == null)
                {
                    if (data.ContainsKey(key))
                        data.Remove(key);
                    return;
                }
                if (data.ContainsKey(key))
                    data[key] = value;
                else
                    data.Add(key, value);
            }
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (IsConnected)
                    Socket.Disconnect(false);
                Socket.Close();
            }
        }

        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        public IPEndPoint RemoteEndPoint
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
        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (IsConnected)
                    return (IPEndPoint)Socket.LocalEndPoint;
                return null;
            }
        }
    }
}
