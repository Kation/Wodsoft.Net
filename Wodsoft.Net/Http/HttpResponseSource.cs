using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Http
{
    public class HttpResponseSource
    {
        public string Charset { get; set; }

        public string ContentType { get; set; }

        public int ContentLength { get; set; }

        public DateTime Date { get; set; }

        public NameValueCollection Headers { get; set; }

        public int StatusCode { get; set; }

        public int SubStatusCode { get; set; }

        public string Status { get; set; }

        public Stream OutputStream { get; set; }
    }
}
