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
    public abstract class SocketDgramBase<TIn, TOut> : ISocket<TIn, TOut>
    {
        private SocketDataBag _DataBag;

        protected SocketDgramBase(ISocketDgramContext context, ISocketDgramHandler<TIn, TOut> socketHandler)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (socketHandler == null)
                throw new ArgumentNullException("socketHandler");
            Context = context;
            Handler = socketHandler;
            context.Receive += context_Receive;
        }

        private void context_Receive(byte[] data, int length)
        {
            var item = Handler.ConvertReceiveData(data, length);
            if (item != null && ReceiveCompleted != null)
                ReceiveCompleted(this, new SocketEventArgs<TOut>(item, SocketAsyncOperation.ReceiveFrom));
        }

        public ISocketDgramContext Context { get; private set; }

        public ISocketDgramHandler<TIn, TOut> Handler { get; set; }

        public virtual bool IsConnected
        {
            get { return true; }
        }

        #region 发送

        public bool Send(TIn data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            var buffer = Handler.ConvertSendData(data);
            if (Context.Send(buffer, SocketFlags.None))
            {
                if (SendCompleted != null)
                    SendCompleted(this, new SocketEventArgs<TIn>(data, SocketAsyncOperation.SendTo));
                return true;
            }
            return false;
        }

        public async Task<bool> SendAsync(TIn data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            var buffer = Handler.ConvertSendData(data);
            if (await Context.SendAsync(buffer, SocketFlags.None))
            {
                if (SendCompleted != null)
                    SendCompleted(this, new SocketEventArgs<TIn>(data, SocketAsyncOperation.SendTo));
                return true;
            }
            return false;
        }

        #endregion

        #region 接收（无效）

        public TOut Receive()
        {
            throw new NotSupportedException();
        }

        public Task<TOut> ReceiveAsync()
        {
            throw new NotSupportedException();
        }

        public void ReceiveCycle()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region 事件

        public event EventHandler<SocketEventArgs<TOut>> ReceiveCompleted;

        public event EventHandler<SocketEventArgs<TIn>> SendCompleted;

        public event EventHandler<SocketEventArgs> DisconnectCompleted;

        #endregion

        #region 断开

        public abstract void Disconnect();

        public abstract Task DisconnectAsync();

        protected void OnDisconnected()
        {
            if (DisconnectCompleted != null)
                DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
        }
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

        public abstract IPEndPoint RemoteEndPoint { get; }

        public abstract IPEndPoint LocalEndPoint { get; }

        #endregion
    }
}
