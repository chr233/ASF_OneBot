using Newtonsoft.Json;

namespace ASF_OneBot.Data.Events.Message
{
    internal class BaseMessageEvent : BaseEvent
    {
        [JsonProperty("post_type", Required = Required.Default)]
        public string PostType { get; internal set; } = "message";

        [JsonProperty("message_type")]
        public string MessageType { get; internal set; } = "";//v11

        [JsonProperty("message_id")]
        public int MessageID { get; internal set; } = 0;//v11

        [JsonProperty("user_id")]
        public long UserID { get; internal set; } = -1;//v11

        [JsonProperty("message")]
        public string Message { get; internal set; } = "";//v11

        [JsonProperty("raw_message")]
        public string RawMessage { get; internal set; } = "";//v11

        [JsonProperty("font")]
        public int Font { get; internal set; } = 0;//v11

        [JsonIgnore]
        public object Sender { get; internal set; } = null;//v11

    }
}
