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
        private bool _WorkStatus;

        protected SocketProcessContext(Stream source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            Source = source;
            DataBag = new SocketDataBag();
            _Event = new ManualResetEvent(false);
            _WorkStatus = false;
        }

        public Stream Source { get; private set; }

        public dynamic DataBag { get; private set; }

        public bool IsFailed { get; set; }

        public void CheckQueue()
        {
            while (true)
            {
                Monitor.Enter(_Event);
                if (_WorkStatus)
                {
                    Monitor.Exit(_Event);
                    _Event.WaitOne();
                }
                else
                {
                    _Event.Reset();
                    _WorkStatus = true;
                    Monitor.Exit(_Event);
                    return;
                }
            }
        }

        public Task CheckQueueAsync(Action callback)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback();
            });
        }

        public Task CheckQueueAsync<T1>(Action<T1> callback, T1 arg1)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1);
            });
        }
        public Task CheckQueueAsync<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1, arg2);
            });
        }
        public Task CheckQueueAsync<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1, arg2, arg3);
            });
        }
        public Task CheckQueueAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1, arg2, arg3, arg4);
            });
        }
        public Task CheckQueueAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1, arg2, arg3, arg4, arg5);
            });
        }
        public Task CheckQueueAsync<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Task.Run(() =>
            {
                CheckQueue();
                callback(arg1, arg2, arg3, arg4, arg5, arg6);
            });
        }

        public virtual void Reset()
        {
            DataBag.Clear();
            _WorkStatus = false;
            IsFailed = false;
            _Event.Set();
        }

        public void Dispose()
        {
            _Event.Dispose();
        }
    }
}
