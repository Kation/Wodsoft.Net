using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public abstract class SocketHeadContentHandler<TIn, TOut> : ISocketHandler<TIn, TOut>
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
                
        public IAsyncResult BeginReceive(SocketHandlerContext<TIn, TOut> context, AsyncCallback callback, object state)
        {
            //context不能为null
            if (context == null)
                throw new ArgumentNullException("context");

            SocketHandlerAsyncResult<TIn, TOut> result = new SocketHandlerAsyncResult<TIn, TOut>(context, state);

            //初始化SocketHandlerState
            SocketHandlerState<TIn, TOut> handlerState = new SocketHandlerState<TIn, TOut>();
            handlerState.Context = context;
            handlerState.AsyncResult = result;
            handlerState.AsyncCallBack = callback;

            context.ReceiveContext.CheckQueueAsync(BeginReceiveCallback, handlerState);
            return result;
        }

        private void BeginReceiveCallback(SocketHandlerState<TIn, TOut> state)
        {
            bool headCompleted = CheckHeadCompleted(state.Context.ReceiveContext);
            if (headCompleted)
            {
                if (CheckContentCompleted(state.Context.ReceiveContext))
                {
                    state.AsyncResult.IsCompleted = true;
                    if (state.AsyncCallBack != null)
                        state.AsyncCallBack(state.AsyncResult);
                    ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                    return;
                }
                state.Context.ReceiveContext.ByteBuffer = new byte[ContentBufferLength];
            }
            else
                state.Context.ReceiveContext.ByteBuffer = new byte[HeadBufferLength];

            try
            {
                if (headCompleted)
                    state.Context.Stream.BeginRead(state.Context.ReceiveContext.ByteBuffer, 0, state.Context.ReceiveContext.ByteBuffer.Length, EndReadContent, state);
                else
                    state.Context.Stream.BeginRead(state.Context.ReceiveContext.ByteBuffer, 0, state.Context.ReceiveContext.ByteBuffer.Length, EndReadHead, state);
            }
            catch
            {
                state.AsyncResult.IsCompleted = false;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
            }

        }

        private void EndReadHead(IAsyncResult ar)
        {
            SocketHandlerState<TIn, TOut> state = (SocketHandlerState<TIn, TOut>)ar.AsyncState;
            int length;
            try
            {
                length = state.Context.Stream.EndRead(ar);
            }
            catch
            {
                state.AsyncResult.CompletedSynchronously = true;
                state.AsyncResult.IsCompleted = false;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                return;
            }
            long position = state.Context.ReceiveContext.Buffer.Position;
            state.Context.ReceiveContext.Buffer.Position = state.Context.ReceiveContext.Buffer.Length;
            state.Context.ReceiveContext.Buffer.Write(state.Context.ReceiveContext.ByteBuffer, 0, length);
            state.Context.ReceiveContext.Buffer.Position = position;
            if (ProcessReceiveHead(state.Context.ReceiveContext))
            {
                if (CheckContentCompleted(state.Context.ReceiveContext))
                {
                    state.AsyncResult.IsCompleted = true;
                    if (state.AsyncCallBack != null)
                        state.AsyncCallBack(state.AsyncResult);
                    ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                    return;
                }
                state.Context.ReceiveContext.ByteBuffer = new byte[ContentBufferLength];
                try
                {
                    state.Context.Stream.BeginRead(state.Context.ReceiveContext.ByteBuffer, 0, state.Context.ReceiveContext.ByteBuffer.Length, EndReadContent, state);
                }
                catch
                {
                    state.AsyncResult.CompletedSynchronously = true;
                    state.AsyncResult.IsCompleted = false;
                    if (state.AsyncCallBack != null)
                        state.AsyncCallBack(state.AsyncResult);
                    ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                    return;
                }
            }
            else
            {
                try
                {
                    state.Context.Stream.BeginRead(state.Context.ReceiveContext.ByteBuffer, 0, state.Context.ReceiveContext.ByteBuffer.Length, EndReadHead, state);
                }
                catch
                {
                    state.AsyncResult.CompletedSynchronously = true;
                    state.AsyncResult.IsCompleted = false;
                    if (state.AsyncCallBack != null)
                        state.AsyncCallBack(state.AsyncResult);
                    ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                    return;
                }            
            }
        }

        private void EndReadContent(IAsyncResult ar)
        {
            SocketHandlerState<TIn, TOut> state = (SocketHandlerState<TIn, TOut>)ar.AsyncState;
            int length;
            try
            {
                length = state.Context.Stream.EndRead(ar);
            }
            catch
            {
                state.AsyncResult.CompletedSynchronously = true;
                state.AsyncResult.IsCompleted = false;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                return;
            }
            long position = state.Context.ReceiveContext.Buffer.Position;
            state.Context.ReceiveContext.Buffer.Position = state.Context.ReceiveContext.Buffer.Length;
            state.Context.ReceiveContext.Buffer.Write(state.Context.ReceiveContext.ByteBuffer, 0, length);
            state.Context.ReceiveContext.Buffer.Position = position;
            if (ProcessReceiveContent(state.Context.ReceiveContext))
            {
                state.AsyncResult.IsCompleted = true;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
            }
            else
            {
                try
                {
                    state.Context.Stream.BeginRead(state.Context.ReceiveContext.ByteBuffer, 0, state.Context.ReceiveContext.ByteBuffer.Length, EndReadContent, state);
                }
                catch
                {
                    state.AsyncResult.CompletedSynchronously = true;
                    state.AsyncResult.IsCompleted = false;
                    if (state.AsyncCallBack != null)
                        state.AsyncCallBack(state.AsyncResult);
                    ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                    return;
                }
            }
        }

        public TOut EndReceive(IAsyncResult asyncResult)
        {
            SocketHandlerAsyncResult<TIn, TOut> result = asyncResult as SocketHandlerAsyncResult<TIn, TOut>;
            if (result == null)
                throw new InvalidOperationException("异步结果不属于该处理器。");
            TOut value = result.Context.ReceiveContext.Result;
            result.Context.ReceiveContext.Reset();
            return value;
        }

        public TOut Receive(SocketHandlerContext<TIn, TOut> context)
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
                }
            }
            TOut value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        public async Task<TOut> ReceiveAsync(SocketHandlerContext<TIn, TOut> context)
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
                    int length = await context.Stream.ReadAsync(buffer, 0, HeadBufferLength);
                    position = context.ReceiveContext.Buffer.Position;
                    context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                    context.ReceiveContext.Buffer.Write(buffer, 0, length);
                    context.ReceiveContext.Buffer.Position = position;
                    headProcessCompleted = ProcessReceiveHead(context.ReceiveContext);
                }
            }
            bool contentProcessCompleted = CheckContentCompleted(context.ReceiveContext);
            if (!contentProcessCompleted)
            {
                byte[] buffer = new byte[ContentBufferLength];
                long position;
                while (!contentProcessCompleted)
                {
                    int length = await context.Stream.ReadAsync(buffer, 0, ContentBufferLength);
                    position = context.ReceiveContext.Buffer.Position;
                    context.ReceiveContext.Buffer.Position = context.ReceiveContext.Buffer.Length;
                    context.ReceiveContext.Buffer.Write(buffer, 0, length);
                    context.ReceiveContext.Buffer.Position = position;
                    contentProcessCompleted = ProcessReceiveContent(context.ReceiveContext);
                }
            }
            TOut value = context.ReceiveContext.Result;
            context.ReceiveContext.Reset();
            return value;
        }

        protected abstract bool ProcessReceiveHead(SocketReceiveContext<TOut> context);

        protected abstract bool ProcessReceiveContent(SocketReceiveContext<TOut> context);

        public IAsyncResult BeginSend(TIn data, SocketHandlerContext<TIn, TOut> context, AsyncCallback callback, object state)
        {
            //data不能为null
            if (data == null)
                throw new ArgumentNullException("data");
            //context不能为null
            if (context == null)
                throw new ArgumentNullException("context");


            SocketHandlerAsyncResult<TIn, TOut> result = new SocketHandlerAsyncResult<TIn, TOut>(context, state);
            
            SocketHandlerState<TIn, TOut> handlerState = new SocketHandlerState<TIn, TOut>();
            handlerState.Context = context;
            handlerState.AsyncResult = result;
            handlerState.AsyncCallBack = callback;

            context.SendContext.CheckQueueAsync(BeginSendCallback, handlerState, data);

            return result;
        }

        private void BeginSendCallback(SocketHandlerState<TIn, TOut> state, TIn data)
        {
            state.Context.SendContext.Data = data;

            byte[] head = ProcessSendHead(state.Context.SendContext);
            try
            {
                state.Context.Stream.BeginWrite(head, 0, head.Length, EndWriteHead, state);
            }
            catch
            {
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
            }
        }

        private void EndWriteHead(IAsyncResult ar)
        {
            SocketHandlerState<TIn, TOut> state = (SocketHandlerState<TIn, TOut>)ar.AsyncState;
            try
            {
                state.Context.Stream.EndWrite(ar);
            }
            catch
            {
                state.AsyncResult.IsCompleted = false;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                return;
            }
            byte[] content = ProcessSendContent(state.Context.SendContext);
            try
            {
                state.Context.Stream.BeginWrite(content, 0, content.Length, EndWriteContent, state);
            }
            catch
            {
                state.AsyncResult.IsCompleted = false;
                if (state.AsyncCallBack != null)
                    state.AsyncCallBack(state.AsyncResult);
                ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
                return;
            }
        }

        private void EndWriteContent(IAsyncResult ar)
        {
            SocketHandlerState<TIn, TOut> state = (SocketHandlerState<TIn, TOut>)ar.AsyncState;
            try
            {
                state.Context.Stream.EndWrite(ar);
                state.AsyncResult.IsCompleted = true;
            }
            catch
            {
                state.AsyncResult.IsCompleted = false;
            }
            if (state.AsyncCallBack != null)
                state.AsyncCallBack(state.AsyncResult);
            ((AutoResetEvent)state.AsyncResult.AsyncWaitHandle).Set();
        }

        public bool EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            SocketHandlerAsyncResult<TIn, TOut> result = asyncResult as SocketHandlerAsyncResult<TIn, TOut>;
            result.Context.SendContext.Reset();
            return result.IsCompleted;
        }

        public bool Send(TIn data, SocketHandlerContext<TIn, TOut> context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                context.Stream.Write(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                context.Stream.Write(content, 0, content.Length);
            }
            catch
            {
                context.SendContext.Reset();
                return false;
            }
            context.SendContext.Reset();
            return true;
        }

        public async Task<bool> SendAsync(TIn data, SocketHandlerContext<TIn, TOut> context)
        {
            context.SendContext.CheckQueue();
            context.SendContext.Data = data;
            try
            {
                byte[] head = ProcessSendHead(context.SendContext);
                await context.Stream.WriteAsync(head, 0, head.Length);
                byte[] content = ProcessSendContent(context.SendContext);
                await context.Stream.WriteAsync(content, 0, content.Length);
            }
            catch
            {
                context.SendContext.Reset();
                return false;
            }
            context.SendContext.Reset();
            return true;
        }

        protected abstract byte[] ProcessSendHead(SocketSendContext<TIn> context);

        protected abstract byte[] ProcessSendContent(SocketSendContext<TIn> context);

        private class SocketHandlerState<TIn, TOut>
        {
            /// <summary>
            /// 数据
            /// </summary>
            public SocketHandlerContext<TIn, TOut> Context { get; set; }
            /// <summary>
            /// 异步结果
            /// </summary>
            public SocketHandlerAsyncResult<TIn, TOut> AsyncResult { get; set; }
            /// <summary>
            /// 异步回调函数
            /// </summary>
            public AsyncCallback AsyncCallBack { get; set; }
        }
    }
}
