using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASF_OneBot.Data.Events.Message
{
    internal class PrivateMessageEvent : BaseMessageEvent
    {
        [JsonProperty("message_type")]
        public new string MessageType { get; internal set; } = "private";//v11

        [JsonProperty("sender")]
        public new PrivateMsgSender Sender { get; internal set; } = null;//v11

    }



}
