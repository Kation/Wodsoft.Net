using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Http
{
    public class HttpSocketHandler : SocketHeadContentHandler<HttpResponseSource, HttpRequestSource>
    {
        public HttpSocketHandler() : base(512, 512) { }

        protected override bool ProcessReceiveHead(SocketReceiveContext<HttpRequestSource> context)
        {
            int index = context.Buffer.IndexOfValues(13, 10, 13, 10);
            if (index == -1)
                return false;
            byte[] data = new byte[index];
            context.Buffer.Read(data, 0, index);
            context.Buffer.Position += 4;
            string head = Encoding.UTF8.GetString(data);
            string[] items = head.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            int end = items[0].IndexOf(' ');
            context.Result = new HttpRequestSource();
            context.Result.Method = items[0].Substring(0, end);
            index = end + 1;
            end = items[0].IndexOf(' ', index);
            context.Result.Path = items[0].Substring(index, end - index);
            context.Result.Protocol = items[0].Substring(end + 1);
            for (int i = 1; i < items.Length; i++)
            {
                index = items[i].IndexOf(':');
                context.Result.Headers.Add(items[i].Substring(0, index), items[i].Substring(index + 2));
            }
            context.Result.Url = new Uri("http://" + context.Result.Headers["Host"] + context.Result.Path, UriKind.Absolute);
            if (context.Result.Url.Query.Length > 1)
                foreach (var query in context.Result.Url.Query.Substring(1).Split('&'))
                {
                    string[] keyAndValue = query.Split('=');
                    context.Result.QueryString.Add(keyAndValue[0], keyAndValue[1]);
                }
            context.Result.Accepts = context.Result.Headers["Accept"] == null ? new string[] { "*/*" } : context.Result.Headers["Accept"].Split(new string[] { ", " }, StringSplitOptions.None);
            return true;
        }

        protected override bool ProcessReceiveContent(SocketReceiveContext<HttpRequestSource> context)
        {
            context.IsFailed = true;
            return true;
        }

        protected override byte[] ProcessSendHead(SocketSendContext<HttpResponseSource> context)
        {            
            throw new NotImplementedException();
        }

        protected override byte[] ProcessSendContent(SocketSendContext<HttpResponseSource> context)
        {
            throw new NotImplementedException();
        }
    }
}
