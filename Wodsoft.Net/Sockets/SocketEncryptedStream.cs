using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    public class SocketEncryptedStream : AuthenticatedStream
    {
        private readonly static byte[] _ProtocolData = new byte[] { 0xeb, 0x77, 0x20, 0xff };

        public SocketEncryptedStream(Stream innerStream, int keySize)
            : base(innerStream, true)
        {
            if (keySize < 1)
                throw new ArgumentOutOfRangeException("keySize不能小于1。");
            KeySize = keySize;
            _IsAuthenticated = false;
            _IsMutuallyAuthenticated = false;
        }

        public int KeySize { get; private set; }

        public byte[] Keys { get; private set; }

        public int KeyReadOffset { get; private set; }

        public int KeyWriteOffset { get; private set; }

        #region 杂项

        private bool _IsAuthenticated;
        public override bool IsAuthenticated
        {
            get { return _IsAuthenticated; }
        }

        public override bool IsEncrypted
        {
            get { return _IsAuthenticated; }
        }

        private bool _IsMutuallyAuthenticated;
        public override bool IsMutuallyAuthenticated
        {
            get { return _IsMutuallyAuthenticated; }
        }

        private bool _IsServer;
        public override bool IsServer
        {
            get { return _IsServer; }
        }

        public override bool IsSigned
        {
            get { return false; }
        }

        public override bool CanRead
        {
            get { return InnerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return InnerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return InnerStream.CanWrite; }
        }

        public override bool CanTimeout
        {
            get
            {
                return InnerStream.CanTimeout;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return InnerStream.ReadTimeout;
            }
            set
            {
                InnerStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return InnerStream.WriteTimeout;
            }
            set
            {
                InnerStream.WriteTimeout = value;
            }
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return InnerStream.FlushAsync(cancellationToken);
        }

        public override long Length
        {
            get { return InnerStream.Length; }
        }

        public override long Position
        {
            get
            {
                return InnerStream.Position;
            }
            set
            {
                InnerStream.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        #endregion

        public override int Read(byte[] buffer, int offset, int count)
        {
            var length = InnerStream.Read(buffer, offset, count);
            DecryptData(buffer, offset, length);
            return length;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return InnerStream.ReadAsync(buffer, offset, count, cancellationToken).ContinueWith<int>(t =>
            {
                DecryptData(buffer, offset, t.Result);
                return t.Result;
            });
        }
        
        public override int ReadByte()
        {
            int data = InnerStream.ReadByte();
            if (data != -1)
            {
                data ^= Keys[KeyReadOffset];
                KeyReadOffset++;
                if (KeyReadOffset == Keys.Length)
                    KeyReadOffset = 0;
            }
            return data;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            buffer = EncryptData(buffer, offset, count);
            InnerStream.Write(buffer, 0, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            buffer = EncryptData(buffer, offset, count);
            return InnerStream.WriteAsync(buffer, 0, count, cancellationToken);
        }
        
        public override void WriteByte(byte value)
        {
            value ^= Keys[KeyWriteOffset];
            KeyWriteOffset++;
            if (KeyWriteOffset == Keys.Length)
                KeyWriteOffset = 0;
            InnerStream.WriteByte(value);
        }

        protected byte[] EncryptData(byte[] buffer, int offset, int count)
        {
            byte[] newBuffer = buffer.Skip(offset).Take(count).Select(t =>
            {
                byte value = (byte)(t ^ Keys[KeyWriteOffset]);
                KeyWriteOffset++;
                if (KeyWriteOffset == Keys.Length)
                    KeyWriteOffset = 0;
                return value;
            }).ToArray();
            return newBuffer;
        }

        protected void DecryptData(byte[] buffer, int offset, int count)
        {
            int range = offset + count;
            for (int i = offset; i < range; i++)
            {
                buffer[i] ^= Keys[KeyReadOffset];
                KeyReadOffset++;
                if (KeyReadOffset == Keys.Length)
                    KeyReadOffset = 0;
            }
        }


        public void AuthenticateAsClient()
        {
            if (IsAuthenticated)
                throw new InvalidOperationException("已经通过了身份验证。");
            _IsServer = false;
            //接收服务器端发送的协议识别数据
            byte[] protocolData = new byte[_ProtocolData.Length];
            int length = 0;
            while (length != 4)
                length += InnerStream.Read(protocolData, length, protocolData.Length - length);
            //检测协议识别数据是否正确
            for (int i = 0; i < _ProtocolData.Length; i++)
                if (_ProtocolData[i] != protocolData[i])
                    throw new AuthenticationException("使用的加密协议不一致。");

            //生成并发送RS公钥
            var rsa = new RSACryptoServiceProvider(2048);
            var publicKey = rsa.ExportParameters(false);
            InnerStream.Write(publicKey.Exponent, 0, publicKey.Exponent.Length);
            InnerStream.Write(publicKey.Modulus, 0, publicKey.Modulus.Length);

            //接收服务器端发送的加密密钥
            byte[] dataLength = new byte[2];
            length = 0;
            while (length != dataLength.Length)
                length += InnerStream.Read(dataLength, length, dataLength.Length - length);
            byte[] data = new byte[BitConverter.ToUInt16(dataLength, 0)];
            length = 0;
            while (length != data.Length)
                length += InnerStream.Read(data, length, data.Length - length);
            //解密密钥
            Keys = rsa.Decrypt(data, true);
            _IsAuthenticated = true;

            ////通过加密流发送成功信息
            //Write(_SuccessData, 0, _SuccessData.Length);

            ////通过加密流接收成功信息
            //byte[] successData = new byte[_SuccessData.Length];
            //length = 0;
            //while (length != successData.Length)
            //    length += Read(successData, length, successData.Length - length);
            ////检测成功数据是否正确
            //for (int i = 0; i < _SuccessData.Length; i++)
            //    if (_SuccessData[i] != successData[i])
            //        throw new AuthenticationException("验证加密成功信息失败。");
            _IsMutuallyAuthenticated = true;
        }

        public async Task AuthenticateAsClientAsync()
        {
            if (IsAuthenticated)
                throw new InvalidOperationException("已经通过了身份验证。");
            _IsServer = false;
            //接收服务器端发送的协议识别数据
            byte[] protocolData = new byte[_ProtocolData.Length];
            int length = 0;
            while (length != 4)
                length += await InnerStream.ReadAsync(protocolData, length, protocolData.Length - length);
            //检测协议识别数据是否正确
            for (int i = 0; i < _ProtocolData.Length; i++)
                if (_ProtocolData[i] != protocolData[i])
                    throw new AuthenticationException("使用的加密协议不一致。");

            //生成并发送RS公钥
            var rsa = new RSACryptoServiceProvider(2048);
            var publicKey = rsa.ExportParameters(false);
            await InnerStream.WriteAsync(publicKey.Exponent, 0, publicKey.Exponent.Length);
            await InnerStream.WriteAsync(publicKey.Modulus, 0, publicKey.Modulus.Length);

            //接收服务器端发送的加密密钥
            byte[] dataLength = new byte[2];
            length = 0;
            while (length != dataLength.Length)
                length += await InnerStream.ReadAsync(dataLength, length, dataLength.Length - length);
            byte[] data = new byte[BitConverter.ToUInt16(dataLength, 0)];
            length = 0;
            while (length != data.Length)
                length += await InnerStream.ReadAsync(data, length, data.Length - length);
            //解密密钥
            Keys = rsa.Decrypt(data, true);
            _IsAuthenticated = true;

            ////通过加密流发送成功信息
            //await WriteAsync(_SuccessData, 0, _SuccessData.Length);

            ////通过加密流接收成功信息
            //byte[] successData = new byte[_SuccessData.Length];
            //length = 0;
            //while (length != successData.Length)
            //    length += await ReadAsync(successData, length, successData.Length - length);
            ////检测成功数据是否正确
            //for (int i = 0; i < _SuccessData.Length; i++)
            //    if (_SuccessData[i] != successData[i])
            //        throw new AuthenticationException("验证加密成功信息失败。");
            _IsMutuallyAuthenticated = true;
        }
        
        public void AuthenticateAsServer()
        {
            if (IsAuthenticated)
                throw new InvalidOperationException("已经通过了身份验证。");
            _IsServer = true;
            //发送协议识别数据
            InnerStream.Write(_ProtocolData, 0, _ProtocolData.Length);
            //接收客户端发送的RSA公钥
            byte[] exponent = new byte[3];
            byte[] modulus = new byte[256];
            int length = 0;
            while (length != exponent.Length)
                length += InnerStream.Read(exponent, length, exponent.Length - length);
            length = 0;
            while (length != modulus.Length)
                length += InnerStream.Read(modulus, length, modulus.Length - length);

            var rsa = new RSACryptoServiceProvider();
            var publicKey = new RSAParameters() { Exponent = exponent, Modulus = modulus };
            rsa.ImportParameters(publicKey);
            GenerateKey();
            //使用RSA加密密钥
            var keys = rsa.Encrypt(Keys, true);
            InnerStream.Write(BitConverter.GetBytes((ushort)keys.Length), 0, 2);
            //发送加密后的密钥
            InnerStream.Write(keys, 0, keys.Length);
            //服务器端验证完成
            _IsAuthenticated = true;

            ////通过加密流接收成功信息
            //byte[] successData = new byte[_SuccessData.Length];
            //length = 0;
            //while (length != successData.Length)
            //    length += Read(successData, length, successData.Length - length);
            ////检测成功数据是否正确
            //for (int i = 0; i < _SuccessData.Length; i++)
            //    if (_SuccessData[i] != successData[i])
            //        throw new AuthenticationException("验证加密成功信息失败。");
            _IsMutuallyAuthenticated = true;


            ////通过加密流发送成功信息
            //Write(_SuccessData, 0, _SuccessData.Length);
        }

        public async Task AuthenticateAsServerAsync()
        {
            if (IsAuthenticated)
                throw new InvalidOperationException("已经通过了身份验证。");
            _IsServer = true;
            //发送协议识别数据
            await InnerStream.WriteAsync(_ProtocolData, 0, _ProtocolData.Length);
            //接收客户端发送的RSA公钥
            byte[] exponent = new byte[3];
            byte[] modulus = new byte[256];
            int length = 0;
            while (length != exponent.Length)
                length += await InnerStream.ReadAsync(exponent, length, exponent.Length - length);
            length = 0;
            while (length != modulus.Length)
                length += await InnerStream.ReadAsync(modulus, length, modulus.Length - length);

            var rsa = new RSACryptoServiceProvider();
            var publicKey = new RSAParameters() { Exponent = exponent, Modulus = modulus };
            rsa.ImportParameters(publicKey);
            GenerateKey();
            //使用RSA加密密钥
            var keys = rsa.Encrypt(Keys, true);
            await InnerStream.WriteAsync(BitConverter.GetBytes((ushort)keys.Length), 0, 2);
            //发送加密后的密钥
            await InnerStream.WriteAsync(keys, 0, keys.Length);
            //服务器端验证完成
            _IsAuthenticated = true;

            ////通过加密流接收成功信息
            //byte[] successData = new byte[_SuccessData.Length];
            //length = 0;
            //while (length != successData.Length)
            //    length += await ReadAsync(successData, length, successData.Length - length);
            ////检测成功数据是否正确
            //for (int i = 0; i < _SuccessData.Length; i++)
            //    if (_SuccessData[i] != successData[i])
            //        throw new AuthenticationException("验证加密成功信息失败。");
            _IsMutuallyAuthenticated = true;

            ////通过加密流发送成功信息
            //await WriteAsync(_SuccessData, 0, _SuccessData.Length);
        }
        
        private void GenerateKey()
        {
            Random rnd = new Random();
            Keys = new byte[KeySize];
            rnd.NextBytes(Keys);
        }
    }
}
