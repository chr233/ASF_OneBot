using ASF_OneBot.Storage;
using System;

namespace ASF_OneBot
{
    internal static class Global
    {
        public static Config? GlobalConfig { get; internal set; }

        public static Version? Version { get; } = typeof(ASF_OneBot).Assembly.GetName().Version;

        public static string Platform { get; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        public static string Implement { get; } = nameof(ASF_OneBot);

        public static double TimeStamp => DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
    }
}
