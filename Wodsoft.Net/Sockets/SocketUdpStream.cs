using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketUdpStream : Stream
    {
        public SocketUdpStream(Socket socket, EndPoint endPoint)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");
            Socket = socket;
            EndPoint = endPoint;
        }

        public Socket Socket { get; private set; }

        public EndPoint EndPoint { get; private set; }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush() { throw new NotSupportedException(); }

        public override long Length { get { throw new NotSupportedException(); } }

        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            EndPoint endpoint = EndPoint;
            return Socket.ReceiveFrom(buffer, offset, count, SocketFlags.None, ref endpoint);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            EndPoint endpoint = EndPoint;
            return Task.Factory.FromAsync<int>(Socket.BeginReceiveFrom(buffer, offset, count, SocketFlags.None, ref endpoint, null, null), EndReceiveFromMethod);
        }

        private int EndReceiveFromMethod(IAsyncResult ar)
        {
            EndPoint endpoint = EndPoint;
            return Socket.EndReceiveFrom(ar, ref endpoint);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            EndPoint endpoint = EndPoint;
            return Socket.BeginReceiveFrom(buffer, offset, count, SocketFlags.None, ref endpoint, null, null);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return EndReceiveFromMethod(asyncResult);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Socket.SendTo(buffer, offset, count, SocketFlags.None, EndPoint);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            await Task.Factory.FromAsync<int>(Socket.BeginSendTo(buffer, offset, count, SocketFlags.None, EndPoint, null, null), Socket.EndSendTo);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Socket.BeginSendTo(buffer, offset, count, SocketFlags.None, EndPoint, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            Socket.EndSendTo(asyncResult);
        }
    }
}
