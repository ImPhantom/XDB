using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using System;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Services;

namespace XDB.Utilities
{
    public class ModUtil
    {
        public static async Task TempBanUserAsync(SocketGuildUser user, SocketCommandContext context, TimeSpan length, string reason)
        {
            try
            {
                var dm = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":hammer:  **{context.User.Username}#{context.User.Discriminator}** has banned `{user.Username}#{user.Discriminator}` for __{length.Humanize()}__\n**Reason:** `{reason}`");
                await dm.SendMessageAsync($":hammer:  You have been temporarily banned from **{context.Guild.Name}** for: `{length.Humanize()}`\n**Reason:** {reason}");
                var ban = new TempBan()
                {
                    GuildId = context.Guild.Id,
                    BannedUserId = user.Id,
                    AdminUserId = context.User.Id,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow,
                    UnbanTime = DateTime.UtcNow.Add(length)
                };
                TempBanService.AddTemporaryBan(ban);
                CheckingService.TempBans.Add(ban);
                await context.Guild.AddBanAsync(user);
            } catch (Exception e)
            {
                BetterConsole.LogError("Moderation", e.Message);
            }
            finally
            {
                await context.Message.DeleteAsync();
                var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                await TimedMessage(reply);
            }
        }

        public static async Task KickUserAsync(SocketGuildUser user, SocketCommandContext context, string reason)
        {
            try
            {
                var dm = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":heavy_check_mark:  **{context.User.Username}** has kicked {user.Mention}\n**Reason:** `{reason}`");
                await dm.SendMessageAsync($":small_blue_diamond: You were kicked from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                await user.KickAsync().ConfigureAwait(false);
                
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Moderation", e.Message);
            }
            finally
            {
                await context.Message.DeleteAsync();
                var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                await TimedMessage(reply);
            }
        }

        public static async Task BanUserAsync(SocketGuildUser user, SocketCommandContext context, string reason)
        {
            var dm = await user.GetOrCreateDMChannelAsync();
            try
            {
                if (string.IsNullOrEmpty(reason))
                {
                    await Logging.TryLoggingAsync($":hammer: **{context.User.Username}** has banned {user.Mention}\n**Reason:** `N/A`");
                    await dm.SendMessageAsync($":small_blue_diamond: You were banned from **{context.Guild.Name}**\n**Reason:** `N/A`");
                } else {
                    await Logging.TryLoggingAsync($":hammer: **{context.User.Username}** has banned {user.Mention}\n**Reason:** `{reason}`");
                    await dm.SendMessageAsync($":small_blue_diamond: You were banned from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                }
                await context.Guild.AddBanAsync(user);
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Moderation", e.Message);
            }
            finally
            {
                await context.Message.DeleteAsync();
                var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                await TimedMessage(reply);
            }
        }

        private static async Task TimedMessage(IMessage message, int ms = 2500)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
