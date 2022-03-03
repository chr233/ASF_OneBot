using ASF_OneBot.Localization;
using ASF_OneBot.Storage;
using Fleck;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using static ASF_OneBot.Utils;

using ASF_OneBot.API.Callback;

namespace ASF_OneBot.API
{
    [Export]
    internal static class WebSocketHost
    {
        internal static WebSocketServer? WsServer = null;

        static private List<IWebSocketConnection> Sockets = new List<IWebSocketConnection>();

        static private int ClientCount => Sockets.Count;

        static private string accessToken => Global.GlobalConfig?.AccessToken;
        /// <summary>
        /// WebSocket接口鉴权
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        internal static bool CheckAuthorizationHeader(IWebSocketConnection socket)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return true;
            }

            if (socket.ConnectionInfo.Headers.TryGetValue("Authorization", out string authBanner))
            {
                return authBanner?[7..] == accessToken;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 启动正向WebSocket服务
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static async Task Start(SocketConfig config)
        {
            if (WsServer != null)
            {
                return;
            }

            ASFLogger.LogGenericInfo("初始化正向WebSocket主机", nameof(WebSocketHost));

            FleckLog.LogAction = (level, message, ex) => {
                switch (level)
                {
                    case LogLevel.Debug:
                        ASFLogger.LogGenericDebug(message, nameof(WebSocketHost));
                        break;
                    case LogLevel.Error:
                        ASFLogger.LogGenericError(message, nameof(WebSocketHost));
                        break;
                    case LogLevel.Warn:
                        ASFLogger.LogGenericWarning(message, nameof(WebSocketHost));
                        break;
                    default:
                        ASFLogger.LogGenericInfo(message, nameof(WebSocketHost));
                        break;
                }
            };

            WebSocketServer server = new($"ws://{config.Host}:{config.Port}") { RestartAfterListenError = true };

            server.Start(socket => {
                if (CheckAuthorizationHeader(socket))
                {
                    Sockets.Add(socket);
                    ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WSClientConnected, socket.GetHashCode(), ClientCount));
                    socket.OnMessage = async (s) => {
                        await SocketOnMsg.OnMessage(socket, s).ConfigureAwait(false);
                    };

                    socket.OnClose = () => {
                        if (Sockets.Contains(socket))
                        {
                            Sockets.Remove(socket);
                        }

                        ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WSClientDisconnected, socket.GetHashCode(), ClientCount));
                    };
                }
                else
                {
                    ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.WSClientAuthFailed, socket.GetHashCode(), ClientCount));

                    socket.Close();
                }




            });

            ASFLogger.LogGenericInfo($"CQHTTP WS主机已经开始在ws://{config.Host}:{config.Port}上监听。");

        }

        internal static async Task Stop()
        {
            if (WsServer == null)
            {
                return;
            }

            WsServer.Dispose();
        }


    }
}
