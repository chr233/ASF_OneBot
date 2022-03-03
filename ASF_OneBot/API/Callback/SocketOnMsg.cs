using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json;
using ASF_OneBot.API.Data;

namespace ASF_OneBot.API.Callback
{
    internal static class SocketOnMsg

    {
        public static async Task OnMessage(IWebSocketConnection socket, string raw)
        {
            try
            {
                BaseRequest request = JsonConvert.DeserializeObject<BaseRequest>(raw);

                string echo = request.Echo;
                string action = request.Action;
                object param = request.Params;
            }
            catch (Exception e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.InternalHandlerError,
                    Message = $"Something went wrong {e}",
                    Data = null
                };

                string json = JsonConvert.SerializeObject(errorResponse);

                await socket.Send(json).ConfigureAwait(false);
                return;
            }


            //await socket.Send(token.ToString()).ConfigureAwait(false);
        }
    }
}
