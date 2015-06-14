using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Test
{
    class Program
    {
        private static MemoryStream stream;

        static void Main(string[] args)
        {
            //stream = new MemoryStream();
            //SocketHandler16 handler = new SocketHandler16();
            //byte[] data = new byte[10000];
            //data[9999] = 255;
            //handler.BeginSend(data, stream, EndSend, handler);
            //Console.ReadLine();
        }

        private static void EndSend(IAsyncResult ar)
        {
            SocketHandler16 handler = (SocketHandler16)ar.AsyncState;
            Console.WriteLine(handler.EndSend(ar));
            var data = stream.ToArray();
            Console.WriteLine(data.Length);
        }
    }
}
