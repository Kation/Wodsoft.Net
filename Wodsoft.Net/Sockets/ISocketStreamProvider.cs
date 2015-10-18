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
    }
}
