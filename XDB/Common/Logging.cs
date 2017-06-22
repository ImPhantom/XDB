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
            if (LogChannelExists())
            {
                var log = Program.client.GetChannel(LogChannel) as SocketTextChannel;
                await log.SendMessageAsync(message);
            }
            else
                Console.WriteLine(Xeno.LoggerFailed);
        }

        private static bool LogChannelExists()
        {
            if (LogChannel != 0)
                return true;
            else
                return false;
        }

        public static async Task LogNicknamesAsync(SocketGuildUser before, SocketGuildUser after)
        {
            if (before.Nickname == null)
                await Logging.TryLoggingAsync($":white_small_square: **{before.Username}#{before.Discriminator}** has changed their nickname:\n`{before.Username}` >> `{after.Nickname}`");
            else if (after.Nickname == null)
                await Logging.TryLoggingAsync($":black_small_square:  **{before.Username}#{before.Discriminator}** has removed their nickname.");
            else
                await Logging.TryLoggingAsync($":white_small_square: **{before.Username}#{before.Discriminator}** has changed their nickname:\n`{before.Nickname}` >> `{after.Nickname}`");
        }
    }
}
