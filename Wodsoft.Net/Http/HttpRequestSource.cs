using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Http
{
    public class HttpRequestSource
    {
        public HttpRequestSource()
        {
            Form = new NameValueCollection();
            Headers = new NameValueCollection();
            QueryString = new NameValueCollection();
        }

        public string[] Accepts { get; set; }

        public NameValueCollection Form { get; set; }

        public NameValueCollection Headers { get; set; }

        public string Hostname { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string Protocol { get; set; }

        public Uri Url { get; set; }

        public NameValueCollection QueryString { get; set; }
    }
}
