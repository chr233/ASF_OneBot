using ArchiSteamFarm.Steam;
using ASF_OneBot.Storage;
using System;
using System.Collections.Generic;
using Fleck;

namespace ASF_OneBot
{
    internal static class Global
    {
        internal static Config? GlobalConfig { get; set; }

        internal static Version? Version { get; } = typeof(ASF_OneBot).Assembly.GetName().Version;

        internal static string Platform { get; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        internal static string Implement { get; } = nameof(ASF_OneBot);

        internal static double TimeStamp => DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;

        /// <summary>正向WS模式 在线Bot列表</summary>
        internal static Dictionary<ulong, Tuple<Bot, IWebSocketConnection?>> WsOnlineBots = new();

        /// <summary>反向WS模式 在线Bot列表</summary>
        internal static Dictionary<ulong, Bot> ReWsOnlineBots = new();
    }
}
