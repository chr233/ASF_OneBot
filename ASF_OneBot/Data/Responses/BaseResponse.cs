using Newtonsoft.Json;
using System;
using System.Composition;

namespace ASF_OneBot.Data.Responses
{
    /// <summary>
    /// 响应基类
    /// </summary>
    [Export]
    public class BaseResponse
    {
        [JsonProperty("status")]
        public string Status { get; internal set; } = "ok";

        [JsonProperty("retcode")]
        public int RetCode { get; internal set; } = 0;

        [JsonProperty("message")]
        public string Message { get; internal set; } = "";

        [JsonProperty("data")]
        public object Data { get; internal set; } = null;

        [JsonProperty("echo")]
        public string Echo { get; internal set; } = Guid.NewGuid().ToString("N");
    }


}
