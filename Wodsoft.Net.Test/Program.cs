using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Http;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //string value = "\r\n";
            //byte[] data = Encoding.UTF8.GetBytes(value);
            //Console.WriteLine(string.Join(" ", data));
            //Console.ReadLine();

            SocketTcpListener<HttpResponseSource, HttpRequestSource> listener = new SocketTcpListener<HttpResponseSource, HttpRequestSource>(new HttpSocketHandler());
            listener.AcceptCompleted += listener_AcceptCompleted;
            listener.Port = 8000;
            listener.Start();
            Console.ReadLine();
        }

        static void listener_AcceptCompleted(object sender, SocketEventArgs<ISocket<HttpResponseSource, HttpRequestSource>> e)
        {
            e.Data.ReceiveAsync().ContinueWith((requestTask) =>
            {
                HttpRequestSource request = requestTask.Result;
                //"C:\\" +  request.Path.Replace("/", "\\")
                HttpResponseSource response = new HttpResponseSource();
                //response.ContentType = "text/html";
                
                e.Data.Send(response);
                e.Data.Disconnect();
            });
        }
    }
}
