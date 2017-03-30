using Discord.WebSocket;
using System.Threading.Tasks;
using XDB.Common.Types;
using System;

namespace XDB.Utilities
{
    public class Logging
    {
        private static ulong LogChannel = Config.Load().LogChannel;

        public static async Task TryLoggingAsync(string message)
        {
            if(LogChannelExists())
            {
                var log = Program.client.GetChannel(LogChannel) as SocketTextChannel;
                await log.SendMessageAsync(message);
            } else
            {
                Console.WriteLine("[Logging] [Error] Log message failed to send! Set your Logging Channel ID in config.json!");
            }
        }

        private static bool LogChannelExists()
        {
            if (LogChannel != 0)
                return true;
            else
                return false;
        }
    }
}
