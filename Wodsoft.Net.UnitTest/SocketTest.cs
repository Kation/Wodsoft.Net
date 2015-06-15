using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.UnitTest
{
    [TestClass]
    public class SocketTest
    {
        [TestMethod]
        public void SocketSendReceiveTest()
        {            
            SocketHandler16 socketHandler = new SocketHandler16();
            SocketTcpListener<byte[], byte[]> listener = new SocketTcpListener<byte[], byte[]>(socketHandler);
            listener.Port = 7000;
            listener.AcceptCompleted += listener_AcceptCompleted;
            listener.Start();
            Assert.IsTrue(listener.IsStarted);

            SocketTcpClient<byte[], byte[]> client = new SocketTcpClient<byte[], byte[]>(socketHandler);
            Assert.IsFalse(client.IsConnected);
            client.DisconnectCompleted += client_DisconnectCompleted;
            byte[] data = new byte[] { 100, 255, 80, 64 };
            Assert.IsTrue(client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000)));
            Assert.IsTrue(client.IsConnected);
            client.ReceiveCycle();
            Assert.IsTrue(client.Send(data));

            listener.Stop();
            Assert.IsFalse(listener.IsStarted);
        }

        void client_DisconnectCompleted(object sender, SocketEventArgs e)
        {
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.DisconnectCompleted -= client_DisconnectCompleted;
        }

        void listener_AcceptCompleted(object sender, SocketEventArgs<ISocket<byte[], byte[]>> e)
        {
            e.Data.ReceiveCompleted += Data_ReceiveCompleted;
            e.Data.ReceiveCycle();
        }

        void Data_ReceiveCompleted(object sender, SocketEventArgs<byte[]> e)
        {
            Assert.AreEqual(4, e.Data.Length);
            Assert.AreEqual(100, e.Data[0]);
            Assert.AreEqual(255, e.Data[0]);
            Assert.AreEqual(80, e.Data[0]);
            Assert.AreEqual(64, e.Data[0]);
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.Disconnect();
            Assert.IsFalse(socket.IsConnected);
            socket.ReceiveCompleted -= Data_ReceiveCompleted;
        }
    }
}
