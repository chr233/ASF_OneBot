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

        /// <summary>
        /// 白名单模式 (默认只监听 BotNameList 中的 Bot)
        /// </summary>
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
        internal SocketConfig WSConfig { get; private set; } = new() { Enable = false, Host = "127.0.0.1", Port = 6700 };

        /// <summary>反向WebSocket服务器配置</summary>
        [JsonProperty(Required = Required.DisallowNull)]
        internal SocketConfig ReWSConfig { get; private set; } = new() { Enable = false, Host = "127.0.0.1", Port = 6800 };

        [JsonConstructor]
        internal Config() { }
    }

    /// <summary>接口配置</summary>
    internal sealed class SocketConfig
    {
        /// <summary>启用服务</summary>
        public bool Enable;
        /// <summary>主机</summary>
        public string Host;
        /// <summary>端口</summary>
        public int Port;


    }
}
