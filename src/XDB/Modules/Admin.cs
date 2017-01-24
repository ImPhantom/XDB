using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Common.Types;

namespace XDB.Modules
{
    public class Admin : ModuleBase
    {
        [Command("cleanup")]
        [Name("cleanup `<num>` (limit 100)")]
        [Remarks("Cleans up specified amount of messages from channel.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Cleanup(int amt)
        {
            if (amt < 1)
                return;
            await Context.Message.DeleteAsync().ConfigureAwait(false);
            int lim = (amt < 100) ? amt : 100;
            var messages = (await Context.Channel.GetMessagesAsync(limit: lim).Flatten().ConfigureAwait(false));
            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("cleanup")]
        [Name("cleanup `<@user>` `<num>` (limit 100)")]
        [Remarks("Cleans up specified amount of messages from channel.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Cleanup(IGuildUser user, int amt)
        {
            if (amt < 1)
                return;

            if (user.Id == Context.User.Id)
                amt += 1;

            int lim = (amt < 100) ? amt : 100;
            var messages = (await Context.Channel.GetMessagesAsync(limit: lim).Flatten()).Where(m => m.Author == user);
            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("kick")]
        [Name("kick `<@user>`")]
        [Remarks("Kicks user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Kick(IGuildUser user)
        {
            try
            {
                var log = await Context.Guild.GetChannelAsync(Config.Load().LogChannel) as ITextChannel;
                if (log == null)
                    await ReplyAsync($":grey_exclamation: {Context.Message.Author.Mention} has kicked {user.Mention}\n**Reason:** `N/A`");
                else
                    await log.SendMessageAsync($":grey_exclamation: {Context.Message.Author.Mention} has kicked {user.Mention}\n**Reason:** `N/A`");
                await user.KickAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("kick")]
        [Name("kick `<@user>` `<reason>`")]
        [Remarks("Kicks user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Kick(IGuildUser user, [Remainder] string str)
        {
            try
            {
                var log = await Context.Guild.GetChannelAsync(Config.Load().LogChannel) as ITextChannel;
                if (log == null)
                    await ReplyAsync($":grey_exclamation: {Context.Message.Author.Mention} has kicked {user.Mention}\n**Reason:** `{str}`");
                else
                    await log.SendMessageAsync($":grey_exclamation: {Context.Message.Author.Mention} has kicked {user.Mention}\n**Reason:** `{str}`");
                await user.KickAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("ban")]
        [Name("ban `<@user>`")]
        [Remarks("Bans user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ban(IGuildUser user)
        {
            try
            {
                var log = await Context.Guild.GetChannelAsync(Config.Load().LogChannel) as ITextChannel;
                if (log == null)
                    await ReplyAsync($":grey_exclamation: {Context.Message.Author.Mention} has banned {user.Mention}\n**Reason:** `N/A`");
                else
                    await log.SendMessageAsync($":grey_exclamation: {Context.Message.Author.Mention} has banned {user.Mention}\n**Reason:** `N/A`");
                await Context.Guild.AddBanAsync(user).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("ban")]
        [Name("ban `<@user>` `<reason>`")]
        [Remarks("Bans user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ban(IGuildUser user, [Remainder] string str)
        {
            try
            {
                var log = await Context.Guild.GetChannelAsync(Config.Load().LogChannel) as ITextChannel;
                if (log == null)
                    await ReplyAsync($":grey_exclamation: {Context.Message.Author.Mention} has banned {user.Mention}\n**Reason:** `{str}`");
                else
                    await log.SendMessageAsync($":grey_exclamation: {Context.Message.Author.Mention} has banned {user.Mention}\n**Reason:** `{str}`");
                await Context.Guild.AddBanAsync(user).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
