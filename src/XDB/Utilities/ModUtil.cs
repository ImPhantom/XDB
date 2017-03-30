using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Utilities
{
    public class ModUtil
    {
        public static async Task KickUserAsync(SocketGuildUser user, CommandContext context, string reason)
        {
            try
            {
                var dm = await user.CreateDMChannelAsync();
                if (string.IsNullOrEmpty(reason))
                {
                    await Logging.TryLoggingAsync($":grey_exclamation: **{context.User.Username}** has kicked {user.Mention}\n**Reason:** `N/A`");
                    await dm.SendMessageAsync($":anger: You were kicked from **{context.Guild.Name}**\n**Reason:** `N/A`");
                } else
                {
                    await Logging.TryLoggingAsync($":grey_exclamation: **{context.User.Username}** has kicked {user.Mention}\n**Reason:** `{reason}`");
                    await dm.SendMessageAsync($":anger: You were kicked from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                }
                await user.KickAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await context.Channel.SendMessageAsync(e.Message);
            }
        }

        public static async Task BanUserAsync(SocketGuildUser user, CommandContext context, string reason)
        {
            var dm = await user.CreateDMChannelAsync();
            try
            {
                if (string.IsNullOrEmpty(reason))
                {
                    await Logging.TryLoggingAsync($":grey_exclamation: **{context.User.Username}** has banned {user.Mention}\n**Reason:** `N/A`");
                    await dm.SendMessageAsync($":anger: You were banned from **{context.Guild.Name}**\n**Reason:** `N/A`");
                } else {
                    await Logging.TryLoggingAsync($":grey_exclamation: **{context.User.Username}** has banned {user.Mention}\n**Reason:** `{reason}`");
                    await dm.SendMessageAsync($":anger: You were banned from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                }
                await context.Guild.AddBanAsync(user);
            }
            catch (Exception e)
            {
                await context.Channel.SendMessageAsync(e.Message);
            }
        }
    }
}
