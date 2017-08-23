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
        public static async Task KickUserAsync(SocketGuildUser user, SocketCommandContext context, string reason)
        {
            try
            {
                var dm = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":heavy_check_mark:  **{context.User.Username}** has kicked {user.Mention}\n**Reason:** `{reason}`");
                await dm.SendMessageAsync($":small_blue_diamond: You were kicked from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                await user.KickAsync($"{reason} (kicked by: {context.User.Username})");
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
                await Logging.TryLoggingAsync($":hammer: **{context.User.Username}** has banned {user.Mention}\n**Reason:** `{reason}`");
                await dm.SendMessageAsync($":small_blue_diamond: You were banned from **{context.Guild.Name}**\n**Reason:** `{reason}`");
                await context.Guild.AddBanAsync(user, reason: $"{reason} (banned by: {context.User.Username})");
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
