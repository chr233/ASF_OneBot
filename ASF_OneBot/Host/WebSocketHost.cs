using ASF_OneBot.Localization;
using ASF_OneBot.Storage;
using Fleck;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using static ASF_OneBot.Utils;
using Newtonsoft.Json;
using System;
using static ASF_OneBot.Host.RequestParser;
using ASF_OneBot.Data.Requests;
using ASF_OneBot.Data.Responses;
using ASF_OneBot.Data;

namespace ASF_OneBot.Host
{
    [Export]
    internal static class WebSocketHost
    {
        internal static WebSocketServer WsServer = null;

        static private List<IWebSocketConnection> Sockets = new List<IWebSocketConnection>();
        static private int ClientCount => Sockets.Count;

        /// <summary>
        /// 启动正向WebSocket服务
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static async Task StartWsServer(SocketConfig config)
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
                if (Authorization.CheckAuthorizationHeader(socket))
                {
                    Sockets.Add(socket);
                    ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WSClientConnected, socket.GetHashCode(), ClientCount));

                    socket.OnMessage = async (msg) => await OnWsMessage(socket, msg).ConfigureAwait(false);
                    socket.OnError = (err) => OnWsError(socket, err);
                    socket.OnClose = () => OnWsClose(socket);
                }
                else
                {
                    ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.WSClientAuthFailed, socket.GetHashCode()));
                    socket.Close((int)RetCodeEnums.RetCode.BadRequest);
                }




            });

            ASFLogger.LogGenericInfo($"CQHTTP WS主机已经开始在ws://{config.Host}:{config.Port}上监听。");

        }

        internal static async Task StopWsServer()
        {
            if (WsServer == null)
            {
                return;
            }

            WsServer.Dispose();
        }


        internal static async Task OnWsMessage(IWebSocketConnection socket, string message)
        {
            string json = null;

            try
            {
                var data = JsonConvert.DeserializeObject<BaseRequest>(message, new JsonRequestConverter());

                string echo = data.Echo;
                string action = data.Action;



                json = echo;

            }
            catch (JsonReaderException e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.BadRequest,
                    Message = "Json dencode failed.",
                    Data = e.ToString()
                };

                json = JsonConvert.SerializeObject(errorResponse);
            }

            catch (Exception e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.InternalHandlerError,
                    Message = "Something went wrong",
                    Data = e.ToString()
                };

                json = JsonConvert.SerializeObject(errorResponse);
            }
            finally
            {
                if (!string.IsNullOrEmpty(json))
                {
                    await socket.Send(json).ConfigureAwait(false);
                }
            }
        }

        internal static void OnWsClose(IWebSocketConnection socket)
        {
            if (Sockets.Contains(socket))
            {
                Sockets.Remove(socket);
            }
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WSClientDisconnected, socket.GetHashCode(), ClientCount));
        }

        internal static void OnWsError(IWebSocketConnection socket, Exception error)
        {
            if (Sockets.Contains(socket))
            {
                Sockets.Remove(socket);
            }
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.WSClientConnectionFailed, socket.GetHashCode(), ClientCount));
        }


    }
}
