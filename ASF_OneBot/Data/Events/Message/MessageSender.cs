using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.Data.Events.Message
{
    public class PrivateMsgSender
    {
        [JsonProperty("user_id")]
        public long UserID { get; internal set; } = -1;

        [JsonProperty("nickname")]
        public string NickName { get; internal set; } = "";

        [JsonProperty("sex")]
        public string Sex { get; internal set; } = "unknown";

        [JsonProperty("age")]
        public int Age { get; internal set; } = 0;
    }

    public class GroupMsgSender : PrivateMsgSender
    {
        [JsonProperty("area")]
        public string Area { get; internal set; } = "";

        [JsonProperty("level")]
        public string Level { get; internal set; } = "";

        [JsonProperty("role")]
        public string Role { get; internal set; } = "";

        [JsonProperty("title")]
        public string Title { get; internal set; } = "";
    }
}
