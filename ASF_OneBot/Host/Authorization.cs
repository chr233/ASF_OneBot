using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.Host
{
    internal static class Authorization
    {
        static private string AccessToken => Global.GlobalConfig?.AccessToken;
        /// <summary>
        /// WebSocket接口鉴权
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        internal static bool CheckAuthorizationHeader(IWebSocketConnection socket)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                return true;
            }

            if (socket.ConnectionInfo.Headers.TryGetValue("Authorization", out string authBanner))
            {
                return authBanner?[7..] == AccessToken;
            }
            else
            {
                return false;
            }
        }

    }
}
