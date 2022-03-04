using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ArchiSteamFarm.IPC.Integration;
using Newtonsoft.Json;
using System.Net;


namespace ASF_OneBot.Storage
{
    /// <summary>应用配置</summary>
    [SuppressMessage("ReSharper", "ClassCannotBeInstantiated")]
    internal sealed class Config
    {
        /// <summary>鉴权Token</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal string AccessToken { get; private set; } = "";

        /// <summary>调试模式</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal bool Debug { get; private set; } = false;

        /// <summary>白名单模式 (默认只监听 BotNameList 中的 Bot)</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal bool WhiteListMode { get; private set; } = true;

        /// <summary>Bot列表</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal ImmutableHashSet<string> BotNameList { get; private set; } = ImmutableHashSet<string>.Empty;

        /// <summary>是否启用心跳</summary>
        [JsonProperty(Required = Required.Default)]
        internal bool EnableHeartbeat { get; private set; } = true;

        /// <summary>心跳间隔</summary>
        [JsonProperty(Required = Required.Default)]
        internal int HeartbeatInterval { get; private set; } = 5000;

        /// <summary>正向WebSocket服务器设置</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal WsSocketConfig WSConfig { get; private set; } = new() { Enable = false, Host = "0.0.0.0", Port = 6700 };

        /// <summary>反向WebSocket服务器配置</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal ReWsSocketConfig ReWSConfig { get; private set; } = new() { Enable = false, Host = "127.0.0.1", Port = 6800 };

        [JsonConstructor]
        internal Config() { }
    }

    /// <summary>WebSocket接口配置</summary>
    internal class WsSocketConfig
    {
        /// <summary>启用服务</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public bool Enable { get; internal set; } = false;

        [JsonProperty(Required = Required.DisallowNull)]
        /// <summary>启用兼容OneBot V11</summary>
        public bool CompatibleWithV11 = true;

        /// <summary>主机</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string Host { get; internal set; } = "0.0.0.0";

        /// <summary>端口</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public int Port { get; internal set; } = 6700;
    }

    /// <summary>反向WebSocket接口配置</summary>
    internal class ReWsSocketConfig
    {
        /// <summary>启用服务</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public bool Enable { get; internal set; } = false;

        /// <summary>主机</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string Host { get; internal set; } = "127.0.0.1";

        /// <summary>端口</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public int Port { get; internal set; } = 6800;

        [JsonProperty(Required = Required.DisallowNull)]
        public string EventPath { get; internal set; } = "/onebot/v11/ws/";

        [JsonProperty(Required = Required.DisallowNull)]
        public string APIPath { get; internal set; } = "/onebot/v11/ws/";
    }
}
