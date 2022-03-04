using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASF_OneBot.Data.Events.Message
{
    internal class GroupMessageEvent : BaseMessageEvent
    {
        [JsonProperty("message_type")]
        public new string MessageType { get; internal set; } = "group";//v11

        [JsonProperty("group_id")]
        public int GroupID { get; internal set; } = 0;//v11

        [JsonProperty("user_id")]

        public object Anymouse { get; internal set; } = null;

        [JsonProperty("sender")]
        public new GroupMsgSender Sender { get; internal set; } = null;//v11


    }



}
