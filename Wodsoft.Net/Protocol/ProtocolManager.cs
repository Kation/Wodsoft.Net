using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Protocol
{
    public class ProtocolManager
    {
        public ProtocolManager()
        {
            Converter = new ProtocolConverter();
        }

        public ProtocolMapping Mapping { get; private set; }

        public ProtocolConverter Converter { get; private set; }

        public ProtocolMapping<T> Map<T>()
            where T : struct
        {
            ProtocolMapping<T> mapping = new ProtocolMapping<T>();
            Mapping = mapping;
            return mapping;
        }

        public void Map(ProtocolMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException("mapping");
            Mapping = mapping;
        }

        public async Task Send(ProtocolSession session, params object[] args)
        {
            List<byte> buffer = new List<byte>();
            foreach (var arg in args)
                buffer.AddRange(Converter.ConverterTo(arg));
            await session.Socket.SendAsync(buffer.ToArray());
        }

        public void Bind(ISocket<byte[], byte[]> socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (Mapping == null)
                throw new NotSupportedException("协议映射未成功配置。");
            if (!socket.IsConnected)
                throw new InvalidOperationException("不支持已断开的Socket连接。");
            ProtocolSession session = new ProtocolSession(socket);
            socket.DataBag.Session = session;
            socket.ReceiveCompleted += socket_ReceiveCompleted;
            socket.DisconnectCompleted += socket_DisconnectCompleted;
            socket.ReceiveCycle();
        }

        public void Unbind(ISocket<byte[], byte[]> socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            socket.ReceiveCompleted -= socket_ReceiveCompleted;
            socket.DisconnectCompleted -= socket_DisconnectCompleted;
        }

        private void socket_DisconnectCompleted(object sender, SocketEventArgs e)
        {
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.DataBag.Session = null;
            Unbind(socket);
        }

        private async void socket_ReceiveCompleted(object sender, SocketEventArgs<byte[]> e)
        {
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            MemoryStream stream = new MemoryStream(e.Data);
            ProtocolContext context = new ProtocolContext(socket.DataBag.Session, stream, Converter);
            await Mapping.Execute(context);
        }
    }
}
