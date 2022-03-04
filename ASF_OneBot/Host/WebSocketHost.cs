using ArchiSteamFarm.Steam;
using ASF_OneBot.Data;
using ASF_OneBot.Data.Responses;
using ASF_OneBot.Exceptions;
using ASF_OneBot.Localization;
using ASF_OneBot.Storage;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using ASF_OneBot.API;
using static ASF_OneBot.Utils;

namespace ASF_OneBot.Host
{
    [Export]
    internal static class WebSocketHost
    {
        static private WebSocketServer? WsServer = null;
        internal static bool IsWsRunning => WsServer != null;
        internal static List<IWebSocketConnection> Sockets { get; private set; } = new List<IWebSocketConnection>();
        static private int ClientCount => Sockets.Count;
        static private WsSocketConfig WsConfig => Global.GlobalConfig.WSConfig;
        static private Dictionary<ulong, Tuple<Bot, IWebSocketConnection>> WsOnlineBots => Global.WsOnlineBots;

        /// <summary>
        /// 启动正向WebSocket服务
        /// </summary>
        internal static async Task StartWsServer()
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

            WebSocketServer server = new($"ws://{WsConfig.Host}:{WsConfig.Port}") { RestartAfterListenError = true };

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

            ASFLogger.LogGenericInfo("正向WebSocket主机已启动");

            if (WsConfig.CompatibleWithV11)
            {
                ASFLogger.LogGenericWarning("兼容模式已开启, 该模式下仅允许上线单个Bot, 如果需要支持多个Bot请等待推出适配器.");
            }

        }

        internal static void StopWsServer()
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
                json = await ApiDispatcher.CallAction(message).ConfigureAwait(false);
            }

            catch (UnsupportAction e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.UnsupportedAction,
                    Message = "Unsupport Action",
                    Data = e.Message,
                    Wording = "暂不支持的 API 动作"
                };

                json = JsonConvert.SerializeObject(errorResponse);
            }

            catch (BotNotFound e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.BadParam,
                    Message = "Target bot not found",
                    Data = e.Message,
                    Wording = "找不到目标Bot, 请考虑打开兼容模式, 或者在请求中带上 self_id 指定Bot"
                };
                json = JsonConvert.SerializeObject(errorResponse);
            }

            catch (JsonReaderException e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.BadRequest,
                    Message = "Json dencode failed",
                    Data = e.Message,
                    Wording = "Json解码失败"
                };
                json = JsonConvert.SerializeObject(errorResponse);
            }
            catch (Exception e)
            {
                BaseResponse errorResponse = new() {
                    Status = "failed",
                    RetCode = (int)RetCodeEnums.RetCode.InternalHandlerError,
                    Message = "Unknown Error",
                    Data = e.Message,
                    Wording = "未知异常"
                };
                ASFLogger.LogNullError(e.ToString());

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
            ASFLogger.LogGenericDebug(error.ToString());
        }
    }
}
