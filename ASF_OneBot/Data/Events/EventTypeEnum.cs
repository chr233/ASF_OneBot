using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.Data.Events
{
    public enum EventType
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

    public class EventTypeHelper
    {
        public static string GetEventTypeString(EventType type)
        {
            return type switch {
                EventType.Message => "message",
                EventType.Notice => "notice",
                EventType.Request => "request",
                EventType.MetaEvent => "meta",
                _ => throw new NotImplementedException()
            };
        }
    }
}
