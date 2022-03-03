﻿using Newtonsoft.Json;
using System.Composition;

namespace ASF_OneBot.API.Data
{
    /// <summary>
    /// 请求基类
    /// </summary>
    [Export]

    public class BaseRequest
    {
        [JsonProperty("action", Required = Required.Always)]
        public string Action { get; internal set; } = "ok";

        //[JsonProperty("params", Required = Required.Always)]
        public object Params { get; internal set; } = null;

        [JsonProperty("echo", Required = Required.Default)]
        public string Echo { get; internal set; } = null;
    }
}
