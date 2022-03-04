using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.Data
{
    public static class RetCodeEnums
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public enum RetCode : int
        {
            /// <summary>成功</summary>
            Success = 0,
            /// <summary>无效的动作请求</summary>
            BadRequest = 10001,
            /// <summary>不支持的动作请求</summary>
            UnsupportedAction = 10002,
            /// <summary>无效的动作请求参数</summary>
            BadParam = 10003,
            /// <summary>不支持的动作请求参数</summary>
            UnsupportedParam = 10004,
            /// <summary>不支持的消息段类型</summary>
            UnsupportedSegment = 10005,
            /// <summary>无效的消息段参数</summary>
            BadSegmentData = 10006,
            /// <summary>不支持的消息段参数</summary>
            UnsupportedSegm = 10007,
            /// <summary>动作处理器实现错误</summary>
            BadHandler = 20001,
            /// <summary>动作处理器运行时抛出异常</summary>
            InternalHandlerError = 20002,
        }
    }
}
