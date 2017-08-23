using Discord;
using Discord.WebSocket;
using Humanizer;
using System;
using System.Linq;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Services;
using XDB.Utilities;

namespace XDB
{
    public class Events
    {
        public static void Listen(CheckingService checking)
        {
            var client = Program.client;

            client.UserJoined += async (user) =>
            {
                if(checking.Mutes.Any(x => x.UserId == user.Id))
                {
                    var muteRole = user.Guild.GetRole(Config.Load().MutedRoleId);
                    await user.AddRoleAsync(muteRole);
                    await user.ModifyAsync(x => x.Mute = true);
                }

                if (Config.Load().Welcome == true)
                {
                    if (user.IsBot)
                        return;
                    var def = user.Guild.DefaultChannel;
                    var message = Config.Load().WelcomeMessage.Replace("{mention}", user.Mention).Replace("{username}", user.Username);
                    await def.SendMessageAsync(message);

                    // Disabled temporarily
                    //var dm = await user.GetOrCreateDMChannelAsync();
                    //await dm.SendMessageAsync(Config.Load().Rules);
                }

                if (Config.Load().ExtraLogging)
                {
                    if (user.IsBot)
                        return;
                    
                    if(AccountAge(user) < TimeSpan.FromDays(1))
                        await Logging.TryLoggingAsync($":white_check_mark: (**New Account**) `{user.Username}#{user.Discriminator}` has joined the server! (Age: {AccountAge(user).Humanize()})");
                    else
                        await Logging.TryLoggingAsync($":white_check_mark:  `{user.Username}#{user.Discriminator}` has joined the server!");
                }

                await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");
            };


            client.UserLeft += async (user) =>
            {
                if (user.IsBot)
                    return;
                if (Config.Load().ExtraLogging)
                {
                    var bans = await user.Guild.GetBansAsync();
                    if (bans.Any(x => x.User.Id == user.Id))
                        return;
                    await Logging.TryLoggingAsync($":x:  `{user.Username}#{user.Discriminator}` has left the server!");
                }
                    
                await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");
            };

            client.MessageReceived += async (message) =>
            {
                if (message.Channel is IDMChannel && message.Author != client.CurrentUser)
                    BetterConsole.LogDM(message);

                if (Config.Load().WordFilter == true)
                {
                    if (message.Author.IsBot)
                        return;
                    if (Config.Load().IgnoredChannels.Contains(message.Channel.Id) || message.Channel is IDMChannel)
                        return;
                    var words = Config.Load().Words;
                    if (words.Any(message.Content.ToLower().Contains))
                    {
                        var log = client.GetChannel(Config.Load().LogChannel) as SocketTextChannel;
                        if (log == null)
                            await message.DeleteAsync();
                        else
                            await log.SendMessageAsync($":anger: {message.Author.Mention} violated the word filter. **Message Deleted**");
                        await message.DeleteAsync();
                    }
                }
            };

            client.MessageDeleted += async (message, channel) =>
            { 
                // Fix ratelimit issue when using the ~clean command
                if (Config.Load().ExtraLogging)
                {
                    if (!message.HasValue)
                        return;
                    var msg = await message.GetOrDownloadAsync();
                    if (msg.Content.Contains("~"))
                        return;
                    if (msg.Author.IsBot)
                        return;
                    if (Config.Load().IgnoredChannels.Contains(channel.Id))
                        return;
                    if (!msg.Attachments.Any())
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: (`#{channel.Name}`) **{msg.Author.Username}** deleted their message:\n{msg.Content.Replace("@", "@\u200B")}");
                    else
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: (`#{channel.Name}`) **{msg.Author.Username}** deleted their message:\n{msg.Content.Replace("@", "@\u200B")}\n{msg.Attachments.FirstOrDefault().Url}");
                }
            };

            client.MessageUpdated += async (before, after, channel) =>
            {
                if (Config.Load().ExtraLogging)
                {
                    var old = await before.GetOrDownloadAsync();
                    if(old.Content != after.Content)
                    {
                        if (after.Author.IsBot)
                            return;
                        if (Config.Load().IgnoredChannels.Contains(channel.Id))
                            return;
                        await Logging.TryLoggingAsync($":heavy_plus_sign: **{after.Author.Username}** edited their message:\n**Before:** {old.Content.Replace("@", "@\u200B")}\n**After:** {after.Content.Replace("@", "@\u200B")}");
                    }
                }
            };

            client.GuildMemberUpdated += async (before, after) =>
            {
                if (Config.Load().ExtraLogging)
                    if (before.Nickname != after.Nickname)
                        await Logging.LogNicknamesAsync(before, after);
            };

            client.UserUpdated += async (before, after) =>
            {
                if (Config.Load().ExtraLogging)
                    if (before.Username != after.Username)
                        await Logging.TryLoggingAsync($":white_small_square: `{before.Username}#{before.Discriminator}` has changed their username to `{after.Username}#{after.Discriminator}`");
            };

            client.ReactionAdded += async (_message, channel, reaction) =>
            {
                if(Config.Load().Owners.Contains(reaction.User.Value.Id))
                {
                    if (!_message.HasValue)
                        return;
                    var message = await _message.GetOrDownloadAsync();
                    var _board = new BoardService(client);
                    await _board.CheckChannelExistence();
                    if (reaction.Emote.Name == "🚫")
                    {
                        var board = new BoardMessage()
                        {
                            Message = message.Content,
                            UserId = message.Author.Id,
                            Timestamp = message.Timestamp
                        };
                        await _board.AddBoardMessageAsync(board);
                        await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    }
                }
            };
        }

        private static TimeSpan AccountAge(SocketUser user)
        {
            var created = user.CreatedAt;
            var today = DateTimeOffset.Now;
            return today - created;
        }
    }
}
