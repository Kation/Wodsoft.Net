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
