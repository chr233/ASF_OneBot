using ArchiSteamFarm.Steam;
using ASF_OneBot.Data.Requests;
using ASF_OneBot.Exceptions;
using ASF_OneBot.Storage;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASF_OneBot.API
{
    internal static class ApiDispatcher
    {
        static private WsSocketConfig WsConfig => Global.GlobalConfig.WSConfig;
        static private Dictionary<ulong, Tuple<Bot, IWebSocketConnection>> WsOnlineBots => Global.WsOnlineBots;

        internal static Tuple<Bot, IWebSocketConnection> WsFetchTargetBot(ulong steamID = 0)
        {
            if (WsOnlineBots.ContainsKey(steamID))
            {
                return WsOnlineBots[steamID];
            }

            if (steamID == 0 && WsConfig.CompatibleWithV11 && WsOnlineBots.Count == 1)
            {
                foreach (var key in WsOnlineBots.Keys)
                {
                    return WsOnlineBots[key];
                }
            }

            throw new BotNotFound(string.Format("找不到机器人 {0}, 可能离线或者未在监视名单中", steamID));
        }

        internal static async Task<string> CallAction(string raw)
        {
            Bot bot = WsFetchTargetBot(0).Item1;

            return await CallAction(bot, raw).ConfigureAwait(false);
        }




        internal static async Task<string> CallAction(Bot bot, string raw)
        {
            JObject json = JObject.Parse(raw);
            string action = json["action"].ToObject<string>();

            switch (action)
            {
                case "send_private_msg":
                    var data = JsonConvert.DeserializeObject<SendPrivateMsgRequest>(raw);
                    return await SendMessage.SendPrivateMsg(bot, data.Params).ConfigureAwait(false);

                //case "send_group_msg":
                //    var data = JsonConvert.DeserializeObject<SendPrivateMsgRequest>(raw);
                //    return await SendMessage.SendPrivateMsg(bot, data.Params).ConfigureAwait(false);
                //case "send_msg":
                //case "send_message":
                //    var data = JsonConvert.DeserializeObject<SendPrivateMsgRequest>(raw);
                //    return await SendMessage.SendPrivateMsg(bot, data.Params).ConfigureAwait(false);
                default:
                    throw new UnsupportAction(action);
            }
        }

    }
}
