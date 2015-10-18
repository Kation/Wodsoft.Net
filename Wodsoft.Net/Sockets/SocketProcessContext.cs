using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketProcessContext : IDisposable
    {
        private ManualResetEvent _Event;
        private Mutex _Mutex;
        private bool _WorkStatus;

        protected SocketProcessContext(Stream source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            Source = source;
            DataBag = new SocketDataBag();
            _Event = new ManualResetEvent(false);
            _WorkStatus = false;
            _Mutex = new Mutex();
        }

        public Stream Source { get; private set; }

        public dynamic DataBag { get; private set; }

        public bool IsFailed { get; set; }

        public void CheckQueue()
        {
            _Mutex.WaitOne();
        }

        public virtual void Reset()
        {
            if (!IsFailed)
                _Mutex.ReleaseMutex();
            DataBag.Clear();
            _WorkStatus = false;
            IsFailed = false;
        }

        public void Dispose()
        {
            _Event.Dispose();
        }
    }
}
