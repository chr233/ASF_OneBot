using ArchiSteamFarm.Steam;
using ASF_OneBot.Data.Requests;
using ASF_OneBot.Data.Responses;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static ASF_OneBot.Utils;

namespace ASF_OneBot.API
{
    internal static class SendMessage
    {
        internal static async Task<string> SendPrivateMsg(Bot bot, SendPrivateMsgParams payload)
        {
            ulong steamID = (ulong)payload.UserId;
            string message = payload.Message;

            bool result = await bot.SendMessage(steamID, message).ConfigureAwait(false);

            ASFLogger.LogGenericInfo(string.Format("发送消息 {0} -> {1} : {2}", bot.BotName, steamID, message));

            SendMessageResponse response = new() {
                Data = new() { MessageID = result ? 1 : 0 }
            };
            return JsonConvert.SerializeObject(response);

        }
        internal static async Task<string> SendGroupMsg(Bot bot, SendPrivateMsgParams payload)
        {
            ulong steamID = (ulong)payload.UserId;
            string message = payload.Message;

            bool result = await bot.SendMessage(steamID, message).ConfigureAwait(false);

            ASFLogger.LogGenericInfo(string.Format("发送消息 {0} -> {1} : {2}", bot.BotName, steamID, message));

            SendMessageResponse response = new() {
                Data = new() { MessageID = result ? 1 : 0 }
            };
            return JsonConvert.SerializeObject(response);
        }
        internal static async Task<string> SendMsg(Bot bot, SendMsgParams payload)
        {
            ulong steamID = (ulong)payload.UserId;
            string message = payload.Message;

            bool result = await bot.SendMessage(steamID, message).ConfigureAwait(false);

            ASFLogger.LogGenericInfo(string.Format("发送消息 {0} -> {1} : {2}", bot.BotName, steamID, message));

            SendMessageResponse response = new() {
                Data = new() { MessageID = result ? 1 : 0 }
            };
            return JsonConvert.SerializeObject(response);
        }
    }
}
