using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASF_OneBot.Data.Events;
using Newtonsoft.Json;

namespace ASF_OneBot.Data.Events.Meta
{
    internal class LifetimeMetaEvent : BaseEvent
    {
        [JsonProperty("interval")]
        public string Lifetime { get; internal set; }

        [JsonProperty("detail_type")]
        public new string DetailType { get; internal set; } = "heartbeat";

        [JsonProperty("interval")]
        public int Interval { get; internal set; } = 5000;

        [JsonProperty("status")]
        public ASFStatus Status { get; internal set; }
    }


    internal class ASFStatus
    {
        [JsonProperty("good")]
        public bool Good { get; internal set; } = true;

        [JsonProperty("online")]
        public bool Online { get; internal set; } = false;
    }
}
