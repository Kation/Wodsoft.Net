using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Http
{
    public class HttpRequestSource
    {
        public HttpRequestSource()
        {
            Cookies = new NameValueCollection();
            Form = new NameValueCollection();
            Headers = new NameValueCollection();
            QueryString = new NameValueCollection();
        }

        public string[] Accepts { get; set; }

        public Encoder ContentEncoding { get; set; }

        public int ContentLength { get; set; }

        public string ContentType { get; set; }

        public NameValueCollection Cookies { get; set; }

        public NameValueCollection Form { get; set; }

        public NameValueCollection Headers { get; set; }

        public Stream InputStream { get; set; }

        public string Hostname { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string Protocol { get; set; }

        public Uri Referer { get; set; }

        public Uri Url { get; set; }

        public NameValueCollection QueryString { get; set; }
    }
}
