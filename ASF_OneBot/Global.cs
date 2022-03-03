using ASF_OneBot.Storage;
using System;

namespace ASF_OneBot
{
    internal static class Global
    {
        public static Config? GlobalConfig { get; internal set; }

        public static Version? Version => typeof(ASF_OneBot).Assembly.GetName().Version;

    }
}
