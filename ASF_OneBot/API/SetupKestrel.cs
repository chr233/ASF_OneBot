using ArchiSteamFarm.Core;
using ArchiSteamFarm.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static ASF_OneBot.Utils;

namespace ASF_OneBot.API
{
    internal class SetupKestrel
    {
        private static IHost? KestrelWebHost;

        internal static async Task Start()
        {
            if (KestrelWebHost != null)
            {
                return;
            }

            // The order of dependency injection matters, pay attention to it
            HostBuilder builder = new();

            string websiteDirectory = Path.Combine(Directory.GetCurrentDirectory(), "OneBot");

            if (!Directory.Exists(websiteDirectory))
            {
                Directory.CreateDirectory(websiteDirectory);
            }

            builder.UseContentRoot(websiteDirectory);

            // Firstly initialize settings that user is free to override
            builder.ConfigureLogging(
             static   logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Global.GlobalConfig.Debug ? LogLevel.Trace : LogLevel.Warning);
                }
            );

            // Enable NLog integration for logging
            builder.UseNLog();

            IPAddress address = IPAddress.Parse(Global.GlobalConfig.WSConfig.Host);

            builder.ConfigureWebHostDefaults(
                webBuilder => {
                    // Set default web root

                    webBuilder.UseWebRoot(websiteDirectory);

                    webBuilder.UseKestrel((builderContext, options) => {
                        options.Listen(address, Global.GlobalConfig.WSConfig.Port);
                    });

                    // Specify Startup class for IPC
                    webBuilder.UseStartup<Startup>();
                }
            );

            // Init history logger for /Api/Log usage
            //Logging.InitHistoryLogger();

            // Start the server
            IHost? kestrelWebHost = null;

            try
            {
                kestrelWebHost = builder.Build();
                await kestrelWebHost.StartAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                ASF.ArchiLogger.LogGenericException(e);
                kestrelWebHost?.Dispose();

                return;
            }

            KestrelWebHost = kestrelWebHost;
            ASFLogger.LogGenericInfo(Strings.IPCReady);
        }

        internal static async Task Stop()
        {
            if (KestrelWebHost == null)
            {
                return;
            }

            await KestrelWebHost.StopAsync().ConfigureAwait(false);
            KestrelWebHost.Dispose();
            KestrelWebHost = null;
        }

    }
}
