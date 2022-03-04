using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ASF_OneBot.Data.Events.Message;
using ASF_OneBot.Host;
using ASF_OneBot.Localization;
using ASF_OneBot.Storage;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using static ASF_OneBot.Utils;

namespace ASF_OneBot
{

    [Export(typeof(IPlugin))]
    internal sealed class ASF_OneBot : IASF, IBotCommand2, IBot, IBotConnection, IBotFriendRequest, IBotMessage
    {
        public string Name => nameof(ASF_OneBot);
        public Version Version => Global.Version;

        [JsonProperty]
        public Config? OneBotConfig => Global.GlobalConfig;

        static private Dictionary<ulong, Tuple<Bot, IWebSocketConnection>> WsOnlineBots => Global.WsOnlineBots;
        static private Dictionary<ulong, Bot> ReWsOnlineBots => Global.ReWsOnlineBots;

        public Task OnLoaded()
        {
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginAbout));

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginVer, nameof(ASF_OneBot), Version.Major, Version.Minor, Version.Build, Version.Revision));
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginContact));

            return Task.CompletedTask;
        }

        public async Task OnASFInit(IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            Config? config = null;

            if (additionalConfigProperties != null)
            {
                foreach ((string configProperty, JToken configValue) in additionalConfigProperties)
                {
                    try
                    {
                        if (configProperty == "ASFOneBotConfig")
                        {
                            config = configValue.ToObject<Config>();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ASFLogger.LogGenericException(e);
                        ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.ReadConfigError));
                    }
                }
            }

            config ??= new();

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.CurrentMode, config.WhiteListMode ? Langs.WhiteListMode : Langs.BlackListMode));

            if (config.WhiteListMode && config.BotNameList.Count == 0)
            {
                ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.NoBotEffected));
            }

            Global.GlobalConfig = config;

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.TextLine));

            if (config.WSConfig.Enable)
            {
                WsSocketConfig wsConfig = config.WSConfig;

                await WebSocketHost.StartWsServer().ConfigureAwait(false);

                ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WebSocketEnabled, wsConfig.Host, wsConfig.Port));
            }

            if (config.ReWSConfig.Enable)
            {
                ReWsSocketConfig reWsConfig = config.ReWSConfig;

                ASFLogger.LogGenericError(string.Format(CurrentCulture, Langs.NotSupportedYet));

                //ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.ReWebSocketEnabled, reWsConfig.Host, reWsConfig.Port));
            }
        }
        public async Task<string> OnBotMessage(Bot bot, ulong steamID, string message)
        {
            PrivateMessageEvent pmEvent = new() {
                Message = message,
                RawMessage = message,
                SelfID = (long)bot.SteamID,
                SubType = "friend",
                Sender = new() {
                    NickName = "未知",
                    UserID = (long)steamID
                }
            };

            //if (WsOnlineBots.TryGetValue(bot.SteamID, out Tuple<Bot, IWebSocketConnection> data))
            //{
            if (WebSocketHost.Sockets.Count > 0)
            {
                IWebSocketConnection? socket = WebSocketHost.Sockets[0];

                string feedback = JsonConvert.SerializeObject(pmEvent);

                await socket.Send(feedback).ConfigureAwait(false);
            }
            //};

            ASFLogger.LogGenericInfo($"{bot.Nickname} {steamID} {message}");

            return "";
        }
        public async Task<string> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0)
        {
            return null;
        }
        public Task OnBotLoggedOn(Bot bot)
        {
            ///<summary>正向连接</summary>
            void AddWs()
            {
                WsSocketConfig wsCfg = OneBotConfig.WSConfig;
                if (wsCfg.Enable)
                {
                    if (WsOnlineBots.Count >= 1)
                    {
                        if (wsCfg.CompatibleWithV11)
                        {
                            ASFLogger.LogGenericWarning(string.Format(CurrentCulture, "当前处于兼容模式, 正向WS只允许上线一个Bot, 已忽略."));
                            return;

                        }
                    }
                    WsOnlineBots[bot.SteamID] = new(bot, null);
                    ASFLogger.LogGenericInfo(string.Format(CurrentCulture, "正向WS: 机器人 {0} 已上线", bot.BotName));
                }
            }
            ///<summary>反向连接</summary>
            void AddReWs()
            {
                ReWsSocketConfig reWsCfg = OneBotConfig.ReWSConfig;
                if (reWsCfg.Enable)
                {
                    //TODO
                }
            }

            if (OneBotConfig.WhiteListMode)
            {
                //白名单模式
                if (OneBotConfig.BotNameList.Contains(bot.BotName))
                {
                    AddWs();
                    AddReWs();
                }
            }
            else
            {
                //黑名单模式
                if (!OneBotConfig.BotNameList.Contains(bot.BotName))
                {
                    AddWs();
                    AddReWs();
                }
            }

            return Task.CompletedTask;
        }

        public Task OnBotDisconnected(Bot bot, EResult reason)
        {
            ulong steamID = bot.SteamID;
            if (WsOnlineBots.ContainsKey(steamID))
            {
                WsOnlineBots.Remove(steamID);
                ASFLogger.LogGenericInfo(string.Format(CurrentCulture, "正向WS: 机器人 {0} 已下线", bot.BotName));
            }
            if (ReWsOnlineBots.ContainsKey(steamID))
            {
                ReWsOnlineBots.Remove(steamID);
                ASFLogger.LogGenericInfo(string.Format(CurrentCulture, "正向WS: 机器人 {0} 已下线", bot.BotName));
                ASFLogger.LogGenericInfo(string.Format(CurrentCulture, "反向WS: 机器人 {0} 已下线", bot.BotName));
            }
            return Task.CompletedTask;
        }

        public Task<bool> OnBotFriendRequest(Bot bot, ulong steamID)
        {
            return Task.FromResult(false);
        }


        public Task OnBotInit(Bot bot)
        {
            return Task.CompletedTask;
        }
        public Task OnBotDestroy(Bot bot)
        {
            return Task.CompletedTask;
        }

    }
}

