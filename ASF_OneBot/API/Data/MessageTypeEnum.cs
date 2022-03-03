using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.API.Data
{
    internal class MessageTypeEnum
    {
        public enum MessageType
        {
            /// <summary>消息</summary>
            Message = 0,
            /// <summary>通知</summary>
            Notice = 1,
            /// <summary>请求</summary>
            Request = 2,
            /// <summary>元事件</summary>
            MetaEvent = 3,
        }
        public static string GetPostTypeString(MessageType msgType)
        {
            return msgType switch {
                MessageType.Message => "message",
                MessageType.Notice => "notice",
                MessageType.Request => "request",
                MessageType.MetaEvent => "meta",
                _ => throw new NotImplementedException()
            };
        }
    }
}
