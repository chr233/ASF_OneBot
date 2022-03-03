
using Newtonsoft.Json;
using System;
using System.Composition;

namespace ASF_OneBot.API.Data
{
    /// <summary>
    /// 事件基类
    /// </summary>
    [Export]
    public class BaseEvent
    {
        [JsonProperty("id", Required = Required.Always)]
        public string ID { get; internal set; }

        [JsonProperty("impl", Required = Required.Always)]
        public string Impl { get; internal set; } = Global.Implement;

        [JsonProperty("platform", Required = Required.Always)]
        public string Platform { get; internal set; } = Global.Platform;

        [JsonProperty("self_id", Required = Required.Always)]
        public string SelfID { get; internal set; }

        [JsonProperty("time", Required = Required.Always)]
        public double Time { get; internal set; } = Global.TimeStamp;

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; internal set; } = "message";

        [JsonProperty("detail_type", Required = Required.Always)]
        public string DetailType { get; internal set; } = "";

        [JsonProperty("sub_type", Required = Required.Always)]
        public string SubType { get; internal set; } = "";
    }

    
}
