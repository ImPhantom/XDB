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
                var log = await context.Guild.GetChannelAsync(Config.Load().LogChannel) as SocketTextChannel;
                var dm = await user.CreateDMChannelAsync();
                if (string.IsNullOrEmpty(reason))
                {
                    if (log == null)
                        await context.Channel.SendMessageAsync($":grey_exclamation: {context.User.Mention} has kicked {user.Mention}\n**Reason:** `N/A`");
                    else
                        await log.SendMessageAsync($":grey_exclamation: {context.User.Mention} has kicked {user.Mention}\n**Reason:** `N/A`");
                    await dm.SendMessageAsync($":anger: You were kicked from **{context.Guild.Name}**\n**Reason:** `N/A`");
                } else {
                    if (log == null)
                        await context.Channel.SendMessageAsync($":grey_exclamation: {context.User.Mention} has kicked {user.Mention}\n**Reason:** `{reason}`");
                    else
                        await log.SendMessageAsync($":grey_exclamation: {context.User.Mention} has kicked {user.Mention}\n**Reason:** `{reason}`");
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
            var log = await context.Guild.GetChannelAsync(Config.Load().LogChannel) as SocketTextChannel;
            var dm = await user.CreateDMChannelAsync();
            try
            {
                if (string.IsNullOrEmpty(reason))
                {
                    if (log == null)
                        await context.Channel.SendMessageAsync($":grey_exclamation: {context.User.Mention} has banned {user.Mention}\n**Reason:** `N/A`");
                    else
                        await log.SendMessageAsync($":grey_exclamation: {context.User.Mention} has banned {user.Mention}\n**Reason:** `N/A`");
                    await dm.SendMessageAsync($":anger: You were banned from **{context.Guild.Name}**\n**Reason:** `N/A`");
                } else {
                    if (log == null)
                        await context.Channel.SendMessageAsync($":grey_exclamation: {context.User.Mention} has banned {user.Mention}\n**Reason:** `{reason}`");
                    else
                        await log.SendMessageAsync($":grey_exclamation: {context.User.Mention} has banned {user.Mention}\n**Reason:** `{reason}`");
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
