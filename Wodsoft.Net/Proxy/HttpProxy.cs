using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Proxy
{
    public class HttpProxy
    {
        private int _Port;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if (IsStarted)
                    throw new InvalidOperationException("已经开始服务后不能修改端口。");
            }
        }

        public bool IsStarted { get; private set; }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
