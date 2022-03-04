using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiSteamFarm.Steam;
using ASF_OneBot.Exceptions;
using static ASF_OneBot.Utils;

namespace ASF_OneBot.API
{
    internal static class SendMessage
    {
        internal static async Task<int> SendMsg(Bot bot, ulong steamID, string message)
        {
            

            bool result = await bot.SendMessage(steamID, message).ConfigureAwait(false);

            ASFLogger.LogGenericInfo(string.Format("发送消息 {0} -> {1} : {2}", bot.BotName, steamID, message));

            return result ? 1 : 0;
        }
    }
}
