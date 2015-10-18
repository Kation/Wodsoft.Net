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
    public class SocketTcpListener<TIn, TOut> : IEnumerable<ISocket<TIn, TOut>>
    {
        protected Socket Socket { get; private set; }

        private HashSet<ISocket<TIn, TOut>> clients;

        public static SocketTcpListener<TIn, TOut> Create<TIn, TOut>(ISocketStreamHandler<TIn, TOut> handler)
        {
            return new SocketTcpListener<TIn, TOut>(handler);
        }

        /// <summary>
        /// 实例化TCP监听者。
        /// </summary>
        public SocketTcpListener(ISocketStreamHandler<TIn, TOut> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            clients = new HashSet<ISocket<TIn, TOut>>();
            Handler = handler;
            IsStarted = false;
            StreamProvider = new SocketNetworkStreamProvider();
        }

        public ISocketStreamHandler<TIn, TOut> Handler { get; private set; }

        private ISocketStreamProvider _StreamProvider;
        public ISocketStreamProvider StreamProvider
        {
            get
            {
                return _StreamProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _StreamProvider = value;
            }
        }

        public int Count { get { return clients.Count; } }

        private int port;
        /// <summary>
        /// 监听端口。
        /// </summary>
        public int Port
        {
            get { return port; }
            set
            {
                if (value < 0 || value > 65535)
                    throw new ArgumentOutOfRangeException(port + "不是有效端口。");
                port = value;
            }
        }

        /// <summary>
        /// 服务启动中
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// 开始服务。
        /// </summary>
        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException("已经开始服务。");
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口
            //可以引发端口被占用异常
            Socket.Bind(new IPEndPoint(IPAddress.Any, port));
            //监听队列
            Socket.Listen(ushort.MaxValue);
            //如果端口是0，则是随机端口，把这个端口赋值给port
            port = ((IPEndPoint)Socket.LocalEndPoint).Port;
            //服务启动中设置为true
            IsStarted = true;
            //开始异步监听
            Socket.BeginAccept(EndAccept, null);
        }

        //异步监听结束
        private async void EndAccept(IAsyncResult result)
        {
            Socket clientSocket = null;

            //获得客户端Socket
            try
            {
                clientSocket = Socket.EndAccept(result);
                Socket.BeginAccept(EndAccept, null);
            }
            catch
            {

            }

            if (clientSocket == null)
                return;

            //实例化客户端类
            ISocket<TIn, TOut> client = await GetClientAsync(clientSocket);
            if (client != null)
            {
                //增加事件钩子
                client.DisconnectCompleted += client_DisconnectCompleted;

                //增加客户端
                lock (clients)
                    clients.Add(client);

                //客户端连接事件
                if (AcceptCompleted != null)
                    AcceptCompleted(this, new SocketEventArgs<ISocket<TIn, TOut>>(client, SocketAsyncOperation.Accept));
            }
        }

        /// <summary>
        /// 停止服务。
        /// </summary>
        public void Stop()
        {
            if (!IsStarted)
                throw new InvalidOperationException("没有开始服务。");
            foreach (var client in clients.ToArray())
            {
                client.Disconnect();
                client.DisconnectCompleted -= client_DisconnectCompleted;
            }
            Socket.Close();
            Socket = null;
            IsStarted = false;
        }

        protected virtual async Task<ISocket<TIn, TOut>> GetClientAsync(Socket socket)
        {
            var client = new SocketTcpClient<TIn, TOut>(socket, Handler, StreamProvider);
            try
            {
                await client.InitializeAsync();
            }
            catch
            {
                return null;
            }
            return client;
        }

        /// <summary>
        /// 接受客户完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs<ISocket<TIn, TOut>>> AcceptCompleted;

        //客户端断开连接
        private void client_DisconnectCompleted(object sender, SocketEventArgs e)
        {
            SocketTcpBase<TIn, TOut> client = (SocketTcpBase<TIn, TOut>)sender;

            //移除客户端
            lock (clients)
                clients.Remove(client);

            client.DisconnectCompleted -= client_DisconnectCompleted;
        }

        /// <summary>
        /// 获取客户端泛型。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ISocket<TIn, TOut>> GetEnumerator()
        {
            return clients.GetEnumerator();
        }

        /// <summary>
        /// 获取客户端泛型。
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return clients.GetEnumerator();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Socket == null)
                return;
            Stop();
        }
    }
}
