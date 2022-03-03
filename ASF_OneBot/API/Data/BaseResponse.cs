using Newtonsoft.Json;
using System.Composition;

namespace ASF_OneBot.API.Data
{
    /// <summary>
    /// 响应基类
    /// </summary>
    [Export]
    public sealed class BaseResponse
    {
        [JsonProperty("status", Required = Required.Always)]
        public string Status { get; private set; } = "ok";

        [JsonProperty("retcode", Required = Required.Always)]
        public int RetCode { get; private set; } = 0;

        [JsonProperty("data", Required = Required.AllowNull)]
        public object Data { get; private set; } = null;

        [JsonProperty("message", Required = Required.Always)]
        public string? Message { get; private set; } = "";

        [JsonProperty("echo", Required = Required.Default)]
        public string Echo { get; private set; } = null;
    }

   
}
