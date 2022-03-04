using Newtonsoft.Json;

namespace ASF_OneBot.Data.Requests
{
    internal class SendMsgRequest : BaseRequest
    {
        [JsonProperty("params", Required = Required.Always)]
        public new SendMsgParams Params { get; set; }

    }
    internal class SendMsgParams
    {
        [JsonProperty("message_type", Required = Required.Default)]
        public string MessageType { get; set; } = "private";

        [JsonProperty("user_id", Required = Required.Default)]
        public long UserId { get; set; }

        [JsonProperty("group_id", Required = Required.Default)]
        public long GroupId { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

        [JsonProperty("auto_escape", Required = Required.Default)]
        public bool AutoEscape { get; set; } = false;
    }
}
