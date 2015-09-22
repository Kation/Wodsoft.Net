using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket缓存流。
    /// </summary>
    public class SocketBufferedStream : Stream
    {
        private int _BufferSize;
        private long _Length;
        private long _Position;
        private List<byte[]> _Buffer;
        private int _CurrentCursor;
        private int _CurrentPosition;
        private int _DirtyLength;

        /// <summary>
        /// 以4096字节大小实例化一个Socket缓存流。
        /// </summary>
        public SocketBufferedStream() : this(4096) { }

        /// <summary>
        /// 初始化Socket缓存流。
        /// </summary>
        /// <param name="bufferSize">缓存块大小。</param>
        public SocketBufferedStream(int bufferSize)
        {
            if (bufferSize < 1)
                throw new ArgumentOutOfRangeException("bufferSize", "Buffer size can not less than 1.");
            _BufferSize = bufferSize;
            _Buffer = new List<byte[]>();
            _Buffer.Add(new byte[bufferSize]);
        }

        /// <summary>
        /// 获取是否支持读取。
        /// 永远返回true。
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// 获取是否支持查找。
        /// 永远返回true。
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// 获取是否支持写入。
        /// 永远返回true。
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// 刷新缓存。无效的方法。
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// 获取缓存有效长度。
        /// </summary>
        public override long Length { get { return _Length; } }

        /// <summary>
        /// 获取缓存当前位置。
        /// </summary>
        public override long Position
        {
            get
            { return _Position; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > _Length)
                    value = _Length;
                _Position = value;
                _CurrentCursor = (int)Math.Floor((value + _DirtyLength) / (double)_BufferSize);
                _CurrentPosition = (int)((value + _DirtyLength) % _BufferSize);
            }
        }

        /// <summary>
        /// 从缓存读取数据。
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移量。</param>
        /// <param name="count">要读取的字节数量。</param>
        /// <returns>成功读取的字节数。</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > _Length - Position)
                count = (int)(_Length - Position);
            int result = count;
            while (count > 0)
            {
                int size = count;
                byte[] currentBuffer = _Buffer[_CurrentCursor];
                if (size > _BufferSize - _CurrentPosition)
                    size = _BufferSize - _CurrentPosition;
                Array.Copy(currentBuffer, _CurrentPosition, buffer, offset, size);
                count -= size;
                offset += size;
                _CurrentPosition += size;
                Position += size;
                if (_CurrentPosition == _BufferSize)
                {
                    _Buffer.Add(new byte[_BufferSize]);
                    _CurrentCursor++;
                    _CurrentPosition = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// 从缓存读取单个字节。
        /// </summary>
        /// <returns>没有数据则返回-1。</returns>
        public override int ReadByte()
        {
            if (_Position == _Length)
                return -1;
            byte[] buffer = _Buffer[_CurrentCursor];
            byte value = buffer[_CurrentPosition];
            _CurrentPosition++;
            _Position++;
            if (_CurrentPosition == _BufferSize)
            {
                _CurrentCursor++;
                _CurrentPosition = 0;
            }
            return value;
        }

        /// <summary>
        /// 寻找缓存位置。
        /// </summary>
        /// <param name="offset">偏移位置。</param>
        /// <param name="origin">相对位置。</param>
        /// <returns>新的流位置。</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _Position = offset;
                    break;
                case SeekOrigin.Current:
                    _Position += offset;
                    break;
                case SeekOrigin.End:
                    _Position = Length + offset;
                    break;
            }
            if (_Position > _Length)
                _Position = _Length;
            if (_Position < 0)
                _Position = 0;
            _CurrentCursor = (int)Math.Floor((_Position + _DirtyLength) / (double)_BufferSize);
            _CurrentPosition = (int)((_Position + _DirtyLength) % _BufferSize);
            return _Position;
        }

        /// <summary>
        /// 设置缓存总大小。
        /// </summary>
        /// <param name="value">缓存大小。</param>
        public override void SetLength(long value)
        {
            int bufferSize = (int)Math.Ceiling((value + _DirtyLength) / (double)_BufferSize);
            for (int i = _Buffer.Count; i < bufferSize; i++)
                _Buffer.Add(new byte[_BufferSize]);
            _Length = value;
        }

        /// <summary>
        /// 向缓存写入数据。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="offset">偏移量。</param>
        /// <param name="count">字节数。</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int size = count;
                byte[] currentBuffer = _Buffer[_CurrentCursor];
                if (size > _BufferSize - _CurrentPosition)
                    size = _BufferSize - _CurrentPosition;
                Array.Copy(buffer, offset, currentBuffer, _CurrentPosition, size);
                count -= size;
                offset += size;
                _CurrentPosition += size;
                _Position += size;
                if (_Position > _Length)
                    _Length = _Position;
                if (_CurrentPosition == _BufferSize)
                {
                    if (_CurrentCursor + 1 == _Buffer.Count)
                        _Buffer.Add(new byte[_BufferSize]);
                    _CurrentCursor++;
                    _CurrentPosition = 0;
                }
            }
        }

        /// <summary>
        /// 向缓存写入单个字节。
        /// </summary>
        /// <param name="value">数据。</param>
        public override void WriteByte(byte value)
        {
            byte[] buffer = _Buffer[_CurrentCursor];
            buffer[_CurrentPosition] = value;
            _CurrentPosition++;
            _Position++;
            if (_Position > _Length)
                _Length = _Position;
            if (_CurrentPosition == _BufferSize)
            {
                if (_CurrentCursor + 1 == _Buffer.Count)
                    _Buffer.Add(new byte[_BufferSize]);
                _CurrentCursor++;
                _CurrentPosition = 0;
            }
        }

        /// <summary>
        /// 将缓存的所有数据转换为字节数据。
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            byte[] data = new byte[_Length];
            long offset = 0;
            for (int i = 0; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                if (i == _Buffer.Count - 1)
                {
                    Array.Copy(buffer, 0, data, offset, _Length % _BufferSize);
                }
                else
                {
                    if (i == 0)
                    {
                        Array.Copy(buffer, _DirtyLength, data, offset, _BufferSize - _DirtyLength);
                        offset += _BufferSize - _DirtyLength;
                    }
                    else
                    {
                        buffer.CopyTo(data, offset);
                        offset += _BufferSize;
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 查找字节位置。
        /// </summary>
        /// <param name="value">要查找的字节。</param>
        /// <returns>字节所在位置，找不到返回-1。</returns>
        public virtual int IndexOf(byte value)
        {
            return IndexOf(value, 0);
        }

        /// <summary>
        /// 查找字节位置。
        /// </summary>
        /// <param name="value">要查找的字节。</param>
        /// <param name="startIndex">开始查找的位置。</param>
        /// <returns>字节所在位置，找不到返回-1。</returns>
        public virtual int IndexOf(byte value, int startIndex)
        {
            if (startIndex < 0 || startIndex >= _Length)
                throw new ArgumentOutOfRangeException("startIndex");
            int bufferIndex = (int)Math.Floor((startIndex + _DirtyLength) / (double)_BufferSize);
            int bufferOffset = (startIndex + _DirtyLength) % _BufferSize;
            int position = startIndex;
            for (int i = bufferIndex; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                while (bufferOffset < _BufferSize)
                {
                    if (buffer[bufferOffset] == value)
                        return position;
                    position++;
                    if (position == Length)
                        goto End;
                    bufferOffset++;
                }
                bufferOffset = 0;
            }
        End:
            return -1;
        }

        /// <summary>
        /// 查找字节位置。
        /// </summary>
        /// <param name="values">要查找的数据。</param>
        /// <returns>数据所在位置，找不到返回-1。</returns>
        public virtual int IndexOfValues(params byte[] values)
        {
            return IndexOfValues(values, 0);
        }

        /// <summary>
        /// 查找字节位置。
        /// </summary>
        /// <param name="values">要查找的数据。</param>
        /// <param name="startIndex">开始查找的位置。</param>
        /// <returns>数据所在位置，找不到返回-1。</returns>
        public virtual int IndexOfValues(byte[] values, int startIndex)
        {
            if (startIndex < 0 || startIndex >= _Length)
                throw new ArgumentOutOfRangeException("startIndex");
            int bufferIndex = (int)Math.Floor((startIndex + _DirtyLength) / (double)_BufferSize);
            int bufferOffset = (startIndex + _DirtyLength) % _BufferSize;
            int position = startIndex;
            int correct = 0;
            for (int i = bufferIndex; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                while (bufferOffset < _BufferSize)
                {
                    if (buffer[bufferOffset] == values[correct])
                    {
                        correct++;
                        if (correct == values.Length)
                            return position - correct + 1;
                    }
                    else
                        correct = 0;
                    position++;
                    if (position == Length)
                        goto End;
                    bufferOffset++;
                }
                bufferOffset = 0;
            }
        End:
            return -1;
        }

        /// <summary>
        /// 关闭缓存流。等同于Clear。
        /// </summary>
        public override void Close()
        {
            Clear();
        }

        /// <summary>
        /// 清空缓存流。
        /// </summary>
        public virtual void Clear()
        {
            _Buffer.RemoveRange(1, _Buffer.Count - 1);
            _CurrentCursor = 0;
            _CurrentPosition = 0;
            _Length = 0;
            _Position = 0;
            _DirtyLength = 0;
        }
        
        /// <summary>
        /// 重置缓存位置。
        /// 该方法会将缓存的当前位置之前的数据删除，当前位置变为0。
        /// </summary>
        public virtual void ResetPosition()
        {
            _Buffer.RemoveRange(0, _CurrentCursor);
            _CurrentCursor = 0;
            _DirtyLength = _CurrentPosition;
            _Length -= _Position;
            _Position = 0;
        }

        /// <summary>
        /// 释放缓存流。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _Buffer.Clear();
            _Buffer = null;
            base.Dispose(disposing);
        }
    }
}
