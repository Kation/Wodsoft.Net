using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.UnitTest
{
    [TestClass]
    public class SocketHandler16Test
    {
        [TestMethod]
        public void WriteTest()
        {
            MemoryStream stream = new MemoryStream();
            SocketStreamHandlerContext<byte[], byte[]> context = new SocketStreamHandlerContext<byte[], byte[]>(stream);
            SocketHandler16 handler = new SocketHandler16();
            byte[] data = new byte[10000];
            data[9999] = 255;
            handler.Send(data, context);
            data = stream.ToArray();
            Assert.AreEqual(10002, data.Length);
            Assert.AreEqual(255, data[10001]);
        }

        [TestMethod]
        public async Task WriteAsyncTest()
        {
            MemoryStream stream = new MemoryStream();
            SocketStreamHandlerContext<byte[], byte[]> context = new SocketStreamHandlerContext<byte[], byte[]>(stream);
            SocketHandler16 handler = new SocketHandler16();
            byte[] data = new byte[10000];
            data[9999] = 255;
            await handler.SendAsync(data, context);
            data = stream.ToArray();
            Assert.AreEqual(10002, data.Length);
            Assert.AreEqual(255, data[10001]);
                        
            data = new byte[10000];
            data[9999] = 255;
            await Task.Factory.FromAsync<bool>(handler.BeginSend(data, context, null, null), handler.EndSend);
            data = stream.ToArray();
            Assert.AreEqual(20004, data.Length);
            Assert.AreEqual(255, data[20003]);
        }

        [TestMethod]
        public void ReadTest()
        {
            MemoryStream stream = new MemoryStream();
            SocketStreamHandlerContext<byte[], byte[]> context = new SocketStreamHandlerContext<byte[], byte[]>(stream);
            SocketHandler16 handler = new SocketHandler16();
            byte[] data = new byte[10000];
            data[9999] = 255;
            handler.Send(data, context);
            stream.Position = 0;
            data = handler.Receive(context);
            Assert.AreEqual(10000, data.Length);
            Assert.AreEqual(255, data[9999]);
        }

        [TestMethod]
        public async Task ReadAsyncTest()
        {
            MemoryStream stream = new MemoryStream();
            SocketStreamHandlerContext<byte[], byte[]> context = new SocketStreamHandlerContext<byte[], byte[]>(stream);
            SocketHandler16 handler = new SocketHandler16();
            byte[] data = new byte[100];
            data[99] = 255;
            await handler.SendAsync(data, context);
            data[99] = 200;
            await handler.SendAsync(data, context);
            data[99] = 100;
            await handler.SendAsync(data, context);
            stream.Position = 0;
            data = await handler.ReceiveAsync(context);
            Assert.AreEqual(255, data[99]);
            data = await handler.ReceiveAsync(context);
            Assert.AreEqual(200, data[99]);
            data = await handler.ReceiveAsync(context);
            Assert.AreEqual(100, data[99]);

            stream.Position = 0;
            data = await Task.Factory.FromAsync<byte[]>(handler.BeginReceive(context, null, null), handler.EndReceive);
            Assert.AreEqual(255, data[99]);

            IAsyncResult ar = handler.BeginReceive(context, null, null);
            Assert.AreEqual(true, ar.CompletedSynchronously);
            data = handler.EndReceive(ar);
            Assert.AreEqual(200, data[99]);

            ar = handler.BeginReceive(context, null, null);
            ar.AsyncWaitHandle.WaitOne();
            Assert.AreEqual(true, ar.CompletedSynchronously);
            data = handler.EndReceive(ar);
            Assert.AreEqual(100, data[99]);
        }
    }
}
