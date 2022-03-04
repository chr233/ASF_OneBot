using Newtonsoft.Json;
using System;
using System.Composition;

namespace ASF_OneBot.Data.Events
{
    /// <summary>
    /// 事件基类
    /// </summary>
    [Export]
    public class BaseEvent
    {
        /// <summary>事件ID v12</summary>
        [JsonProperty("id")]
        public string ID { get; internal set; } = Guid.NewGuid().ToString();

        /// <summary>OneBot 实现名称 v12</summary>
        [JsonProperty("impl")]
        public string Impl { get; internal set; } = Global.Implement; //v12

        ///<summary>OneBot 实现平台名称 v12</summary>
        [JsonProperty("platform")]
        public string Platform { get; internal set; } = Global.Platform;//v12

        /// <summary>机器人自身 ID</summary>
        [JsonProperty("self_id")]
        public long SelfID { get; internal set; } = 0;

        /// <summary>事件发生时间</summary>
        [JsonProperty("time")]
        public double Time { get; internal set; } = Global.TimeStamp;

        /// <summary>事件类型，必须是 meta、message、notice、request 中的一个，分别表示元事件、消息事件、通知事件和请求事件 v12</summary>
        [JsonProperty("type")]
        public string Type { get; internal set; } = "";//v12

        /// <summary>事件详细类型 v12</summary>
        [JsonProperty("detail_type")]
        public string DetailType { get; internal set; } = "";//v12

        /// <summary>事件子类型</summary>
        [JsonProperty("sub_type")]
        public string SubType { get; internal set; } = "";
    }


}
