using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASF_OneBot.Data.Requests
{
    internal class SendGroupMsgRequest : BaseRequest
    {
        [JsonProperty("params", Required = Required.Always)]
        public new SendPrivateMsgParams Params { get; set; }
    }

    internal class SendGroupMsgParams
    {
        [JsonProperty("group_id", Required = Required.Always)]
        public long GroupId { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

        [JsonProperty("auto_escape", Required = Required.Default)]
        public bool AutoEscape { get; set; } = false;
    }

}
