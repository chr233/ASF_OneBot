
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
        public string ID { get; private set; }

        [JsonProperty("impl", Required = Required.Always)]
        public string Impl { get; private set; } = Global.Implement;

        [JsonProperty("platform", Required = Required.Always)]
        public string Platform { get; private set; } = Global.Platform;

        [JsonProperty("self_id", Required = Required.Always)]
        public string SelfID { get; private set; }

        [JsonProperty("time", Required = Required.Always)]
        public double Time { get; private set; } = Global.TimeStamp;

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; private set; } = "message";

        [JsonProperty("detail_type", Required = Required.Always)]
        public string DetailType { get; private set; } = "";

        [JsonProperty("sub_type", Required = Required.Always)]
        public string SubType { get; private set; } = "";
    }

    
}
