using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ArchiSteamFarm.IPC.Integration;
using Newtonsoft.Json;


namespace ASF_OneBot.Storage
{
    /// <summary>
    /// 应用配置。
    /// </summary>
    ///

    [SuppressMessage("ReSharper", "ClassCannotBeInstantiated")]
    internal sealed class GlobalConfig
    {
        /// <summary>
        /// 鉴权Token
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        internal string AccessToken { get; private set; } = "";

        /// <summary>
        /// 白名单模式 (默认只监听 BotNameList 中的 Bot)
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal bool WhiteListMode { get; private set; } = true;

        /// <summary>
        /// Bot列表
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal ImmutableHashSet<string> BotNameList { get; private set; } = ImmutableHashSet<string>.Empty;

        /// <summary>
        /// 正向WebSocket服务器设置
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal WebSocketConfig WSConfig { get; private set; } = new() { Enable = false, Host = "127.0.0.1", Port = 6700 };

        /// <summary>
        /// 反向WebSocket服务器配置
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal WebSocketConfig ReWSConfig { get; private set; } = new() { Enable = false, Host = "127.0.0.1", Port = 6800 };

        [JsonConstructor]
        internal GlobalConfig() { }
    }

    internal sealed class WebSocketConfig
    {
        /// <summary>
        /// 启用WebSockets服务
        /// </summary>
        public bool Enable;
        /// <summary>
        /// WebSockets主机
        /// </summary>
        public string Host;
        /// <summary>
        /// WebSockets端口
        /// </summary>
        public uint Port;
    }
}
