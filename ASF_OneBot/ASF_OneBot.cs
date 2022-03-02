using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ASF_OneBot.Localization;
using ASF_OneBot.Storage;
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
    internal sealed class ASF_OneBot : IASF, IBotCommand2 ,IBot, IBotConnection, IBotFriendRequest, IBotMessage, IBotModules
    {
        public string Name => nameof(ASF_OneBot);
        public Version Version => typeof(ASF_OneBot).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

        internal readonly Dictionary<string, IBot> RegisteredAdapter = new();

        [JsonProperty]
        public GlobalConfig? globalconfig;

        public Task OnLoaded()
        {
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginAbout));

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginVer, nameof(ASF_OneBot), version.Major, version.Minor, version.Build, version.Revision));
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginContact));

            return Task.CompletedTask;
        }

        public Task OnASFInit(IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            

            GlobalConfig? config = null;

            if (additionalConfigProperties != null)
            {
                foreach ((string configProperty, JToken configValue) in additionalConfigProperties)
                {
                    try
                    {
                        if (configProperty == "ASFOneBotConfig")
                        {
                            config = configValue.ToObject<GlobalConfig>();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ASF.ArchiLogger.LogGenericException(e);
                        ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.ReadConfigError));
                    }
                }
            }

            config ??= new GlobalConfig();

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.CurrentMode, config.WhiteListMode ? Langs.WhiteListMode : Langs.BlackListMode));

            if (config.WhiteListMode && config.BotNameList.Count == 0)
            {
                ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.NoBotEffected));
            }

            globalconfig = config;

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.TextLine));
            return Task.CompletedTask;
        }

        public async Task<string> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0)
        {
            return null;
        }

        public Task OnBotDestroy(Bot bot)
        {
            return Task.CompletedTask;
        }

        public Task OnBotDisconnected(Bot bot, EResult reason)
        {
            return Task.CompletedTask;
        }

        public Task<bool> OnBotFriendRequest(Bot bot, ulong steamID)
        {
            return Task.FromResult(true);
        }


        public Task OnBotInit(Bot bot)
        {


            return Task.CompletedTask;
        }

        public async Task OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            bot.ArchiLogger.LogGenericInfo("Pausing this bot as asked from the plugin");
            await bot.Actions.Pause(true).ConfigureAwait(false);
        }

        public Task OnBotLoggedOn(Bot bot)
        {
            return Task.CompletedTask;
        }


        public Task<string> OnBotMessage(Bot bot, ulong steamID, string message)
        {
            bot.ArchiLogger.LogGenericTrace("Hey boss, we got some unknown message here!");
            return Task.FromResult("I didn't get that, did you mean to use a command?");
        }



    }
}

