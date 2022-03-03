using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json;
using ASF_OneBot.API.Data;

namespace ASF_OneBot.API.Event
{
    internal class SocketOnMsg
    {
        public async Task SocketOnMessage(IWebSocketConnection socket, string raw)
        {
            try
            {
                BaseRequest request = JsonConvert.DeserializeObject<BaseRequest>(raw);

                string echo = request.Echo;
                string action = request.Action;
                object param = request.Params;
            }
            catch (Exception)
            {
                await socket.Send(@"{""status"": ""failed"",""retcode"": 1400,\"data\": null}");
                return;
            }

            JToken token = _cqActionHandler.Process(action, payload);
            token["echo"] = echo;
            await socket.Send(token.ToString());
        }
    }
}
