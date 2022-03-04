using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASF_OneBot.Data.Responses
{
    internal class SendMessageResponse : BaseResponse
    {
        [JsonProperty("data")]
        public new SendMessageData Data { get; internal set; }
    }

    internal class SendMessageData
    {
        [JsonProperty("message_id")]
        public int MessageID { get; internal set; }
    }
}
