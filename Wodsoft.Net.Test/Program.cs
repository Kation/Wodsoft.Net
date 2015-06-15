using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Test
{
    class Program
    {
        private static byte[] _Data = new byte[]{
            100,255,80,5
        };

        static void Main(string[] args)
        {
            SocketHandler16 socketHandler = new SocketHandler16();
            SocketTcpListener<byte[], byte[]> listener = new SocketTcpListener<byte[], byte[]>(socketHandler);
            listener.Port = 7000;
            listener.AcceptCompleted += listener_AcceptCompleted;
            listener.Start();

            SocketTcpClient<byte[], byte[]> client = new SocketTcpClient<byte[], byte[]>(socketHandler);
            client.DisconnectCompleted += client_DisconnectCompleted;
            if (client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000)))
            {
                Console.WriteLine("连接服务器成功。");
                if (client.Send(_Data))
                    Console.WriteLine("发送数据成功。");
                else
                    Console.WriteLine("发送数据失败。");
            }
            else
            {
                Console.WriteLine("连接服务器失败。");
            }
            Console.ReadLine();
        }

        static void client_DisconnectCompleted(object sender, SocketEventArgs e)
        {
            Console.WriteLine("服务器断开了连接。");
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.DisconnectCompleted -= client_DisconnectCompleted;
        }

        static void listener_AcceptCompleted(object sender, SocketEventArgs<ISocket<byte[], byte[]>> e)
        {
            e.Data.ReceiveCompleted += Data_ReceiveCompleted;
        }

        static void Data_ReceiveCompleted(object sender, SocketEventArgs<byte[]> e)
        {
            Console.WriteLine("服务器端收到数据：" + string.Join(" ", e.Data));
            ISocket<byte[], byte[]> socket = (ISocket<byte[], byte[]>)sender;
            socket.Disconnect();
            socket.ReceiveCompleted -= Data_ReceiveCompleted;
        }
    }
}
