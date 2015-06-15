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
    public class SocketDataBagTest
    {
        [TestMethod]
        public void AllTest()
        {
            SocketDataBag source = new SocketDataBag();
            dynamic databag = source;
            Assert.AreEqual(0, source.Count);
            databag.Item1 = 100;
            Assert.AreEqual(1, source.Count);
            Assert.AreEqual(100, databag.Item1);
            double item1 = databag.Item1;
            Assert.AreEqual(100.0, item1);
            databag.Item2 = new MemoryStream();
            Assert.AreEqual(2, source.Count);
            Stream item2 = databag.Item2;
            databag.Clear();
            Assert.AreEqual(0, source.Count);
        }
    }
}
