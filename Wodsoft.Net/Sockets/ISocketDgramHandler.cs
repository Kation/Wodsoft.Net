using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket流处理器接口。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public interface ISocketDgramHandler<TIn, TOut>
    {
        /// <summary>
        /// 转换接收到的数据。
        /// </summary>
        /// <param name="data">收到的数据。</param>
        /// <param name="length">有效长度。</param>
        /// <returns></returns>
        TOut ConvertReceiveData(byte[] data, int length);

        /// <summary>
        /// 转换要发送的数据。
        /// </summary>
        /// <param name="obj">发送的数据。</param>
        /// <returns></returns>
        byte[] ConvertSendData(TIn obj);
    }
}
