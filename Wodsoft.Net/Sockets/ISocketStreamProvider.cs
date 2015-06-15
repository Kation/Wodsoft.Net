using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.Net.Sockets
{
    /// <summary>
    /// Socket数据流提供接口。
    /// </summary>
    public interface ISocketStreamProvider
    {
        /// <summary>
        /// 获取数据流。
        /// </summary>
        /// <param name="socket">已连接的Socket。</param>
        /// <returns>Socket数据流。</returns>
        Stream GetStream(Socket socket);
        /// <summary>
        /// 异步获取数据流。
        /// </summary>
        /// <param name="socket">已连接的Socket。</param>
        /// <returns>Socket数据流任务。</returns>
        Task<Stream> GetStreamAsync(Socket socket);
        /// <summary>
        /// 异步开始获取数据流。
        /// </summary>
        /// <param name="socket">已连接的Socket。</param>
        /// <param name="callback">回调方法。</param>
        /// <param name="state">异步状态对象。</param>
        /// <returns>异步结果。</returns>
        IAsyncResult BeginGetStream(Socket socket, AsyncCallback callback, object state);
        /// <summary>
        /// 异步结束获取数据流。
        /// </summary>
        /// <param name="ar">异步结果。</param>
        /// <returns>Socket数据流。</returns>
        Stream EndGetStream(IAsyncResult ar);
    }
}
