using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wodsoft.Net.Sockets;
using System.Net;

namespace Wodsoft.Net.UnitTest
{
    [TestClass]
    public class SocketUdpTest
    {
        private SocketUdpHost<byte[], byte[]> host, host2;

        [TestMethod]
        public void UdpSendReceiveTest()
        {
            SocketSourceHandler socketHandler = new SocketSourceHandler();
            host = new SocketUdpHost<byte[], byte[]>(socketHandler);
            host.AcceptCompleted += listener_AcceptCompleted;
            host.Start(new IPEndPoint(IPAddress.Any, 8000));
            Assert.IsTrue(host.IsStarted);

            host2 = new SocketUdpHost<byte[], byte[]>(socketHandler);
            Assert.IsFalse(host2.IsStarted);
            host2.Start();
            ISocket<byte[], byte[]> client = host2.Open(new IPEndPoint(IPAddress.Loopback, 8000));
            client.DisconnectCompleted += client_DisconnectCompleted;
            byte[] data = new byte[] { 100, 255, 80, 64 };
            Assert.IsTrue(client.IsConnected);
            Assert.IsTrue(client.Send(data));

        }

        void client_DisconnectCompleted(object sender, SocketEventArgs e)
        {
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.DisconnectCompleted -= client_DisconnectCompleted;
        }

        void listener_AcceptCompleted(object sender, SocketEventArgs<ISocket<byte[], byte[]>> e)
        {
            e.Data.ReceiveCompleted += Data_ReceiveCompleted;
        }

        void Data_ReceiveCompleted(object sender, SocketEventArgs<byte[]> e)
        {
            Assert.AreEqual(4, e.Data.Length);
            Assert.AreEqual(100, e.Data[0]);
            Assert.AreEqual(255, e.Data[1]);
            Assert.AreEqual(80, e.Data[2]);
            Assert.AreEqual(64, e.Data[3]);
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.Disconnect();
            Assert.IsFalse(socket.IsConnected);
            socket.ReceiveCompleted -= Data_ReceiveCompleted;
            host.Stop();
            host2.Stop();
            Assert.IsFalse(host.IsStarted);
        }
    }
}
