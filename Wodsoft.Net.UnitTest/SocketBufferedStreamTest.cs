using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.UnitTest
{
    [TestClass]
    public class SocketBufferedStreamTest
    {
        [TestMethod]
        public void WriteTest()
        {
            SocketBufferedStream stream = new SocketBufferedStream();
            byte[] data = new byte[10000];
            data[9998] = 255;
            data[9999] = 128;
            stream.Write(data, 0, data.Length);
            Assert.AreEqual(10000, stream.Length);
            stream.Position = 5000;
            stream.ResetPosition();
            Assert.AreEqual(4998, stream.IndexOfValues(255, 128));
            stream.Position = 5000;
            for (int i = 0; i < 9998; i++)
            {
                stream.WriteByte(33);
            }
            stream.WriteByte(255);
            stream.WriteByte(100);
            Assert.AreEqual(14998, stream.IndexOfValues(255, 100));
        }

        [TestMethod]
        public void ReadTest()
        {
            SocketBufferedStream stream = new SocketBufferedStream();
            byte[] data = new byte[10000];
            data[9998] = 255;
            data[9999] = 128;
            stream.Write(data, 0, data.Length);
            stream.Position = 5000;
            data = new byte[5000];
            stream.ResetPosition();
            stream.Read(data, 0, 5000);
            Assert.AreEqual(4998, stream.IndexOfValues(255, 128));
        }
    }
}
