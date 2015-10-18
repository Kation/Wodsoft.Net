using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public abstract class SocketHeadContentHandler<TIn, TOut> : ISocketStreamHandler<TIn, TOut>
    {
        protected int HeadBufferLength { get; private set; }

        protected int ContentBufferLength { get; private set; }

        protected SocketHeadContentHandler(int headBufferLength, int contentBufferLength)
        {
            HeadBufferLength = headBufferLength;
            ContentBufferLength = contentBufferLength;
        }

        private bool CheckHeadCompleted(SocketReceiveContext<TOut> context)
        {
            if (context.Buffer.Length == 0)
                return false;
            return ProcessReceiveHead(context);
        }

        private bool CheckContentCompleted(SocketReceiveContext<TOut> context)
        {
            if (context.Buffer.Length == 0)
                return false;
            return ProcessReceiveContent(context);
        }
        
        public TOut Receive(SocketStreamHandlerContext<TIn, TOut> context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            context.ReceiveContext.CheckQueue();

            bool headProcessCompleted = CheckHeadCompleted(context.ReceiveContext);
            if (!headProcessCompleted)
            {
                byte[] buffer = new byte[HeadBufferLength];
                long position;
                while (!headProcessCompleted)
                {
                    try
                    {
                        int length = context.Stream.Read(buffer, 0, HeadBufferLength);
                        if (length == 0)
                        {
                            return default(TOut);
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                    headProcessCompleted = ProcessReceiveHead(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                }
            }
            bool contentProcessCompleted = CheckContentCompleted(context.ReceiveContext);
            if (!contentProcessCompleted)
            {
                byte[] buffer = new byte[ContentBufferLength];
                long position;
                while (!contentProcessCompleted)
                {
                    try
                    {
                        int length = context.Stream.Read(buffer, 0, ContentBufferLength);
                        if (length == 0)
                        {
                            return default(TOut);
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                    contentProcessCompleted = ProcessReceiveContent(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                }
            }
            TOut value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        public async Task<TOut> ReceiveAsync(SocketStreamHandlerContext<TIn, TOut> context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            context.ReceiveContext.CheckQueue();

            bool headProcessCompleted = CheckHeadCompleted(context.ReceiveContext);
            if (!headProcessCompleted)
            {
                if (context.ReceiveContext.IsFailed)
                {
                    context.ReceiveContext.Reset();
                    return default(TOut);
                }
                byte[] buffer = new byte[HeadBufferLength];
                long position;
                while (!headProcessCompleted)
                {
                    try
                    {
                        int length = await context.Stream.ReadAsync(buffer, 0, HeadBufferLength);
                        if (length == 0)
                        {
                            return default(TOut);
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                    headProcessCompleted = ProcessReceiveHead(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                }
            }
            bool contentProcessCompleted = CheckContentCompleted(context.ReceiveContext);
            if (!contentProcessCompleted)
            {
                if (context.ReceiveContext.IsFailed)
                {
                    context.ReceiveContext.Reset();
                    return default(TOut);
                }
                byte[] buffer = new byte[ContentBufferLength];
                long position;
                while (!contentProcessCompleted)
                {
                    try
                    {
                        int length = await context.Stream.ReadAsync(buffer, 0, ContentBufferLength);
                        if (length == 0)
                        {
                            return default(TOut);
                        }
                        position = context.ReceiveContext.Buffer.Position;
                        context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                        context.ReceiveContext.Buffer.Write(buffer, 0, length);
                        context.ReceiveContext.Buffer.Position = position;
                    }
                    catch
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                    contentProcessCompleted = ProcessReceiveContent(context.ReceiveContext);
                    if (context.ReceiveContext.IsFailed)
                    {
                        context.ReceiveContext.Reset();
                        return default(TOut);
                    }
                }
            }
            TOut value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        protected abstract bool ProcessReceiveHead(SocketReceiveContext<TOut> context);

        protected abstract bool ProcessReceiveContent(SocketReceiveContext<TOut> context);
        
        public bool Send(TIn data, SocketStreamHandlerContext<TIn, TOut> context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                if (head != null)
                    context.Stream.Write(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                context.Stream.Write(content, 0, content.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
            return true;
        }

        public async Task<bool> SendAsync(TIn data, SocketStreamHandlerContext<TIn, TOut> context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                if (head != null)
                    await context.Stream.WriteAsync(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                await context.Stream.WriteAsync(content, 0, content.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                context.SendContext.Reset();
            }
            return true;
        }

        protected abstract byte[] ProcessSendHead(SocketSendContext<TIn> context);

        protected abstract byte[] ProcessSendContent(SocketSendContext<TIn> context);
    }
}
