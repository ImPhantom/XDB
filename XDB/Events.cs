﻿using Discord.WebSocket;
using System.Linq;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB
{
    public class Events
    {
        public static void Listen()
        {
            var client = Program.client;

            client.UserJoined += async (user) =>
            {
                if (Config.Load().Welcome == true)
                {
                    if (user.IsBot)
                        return;
                    var def = user.Guild.DefaultChannel;
                    var message = Config.Load().WelcomeMessage.Replace("{mention}", user.Mention).Replace("{username}", user.Username);
                    await def.SendMessageAsync(message);
                }

                if (Config.Load().ExtraLogging)
                {
                    if (user.IsBot)
                        return;
                    await Logging.TryLoggingAsync($":white_check_mark:  `{user.Username}#{user.Discriminator}` has joined the server!");
                }
            };


            client.UserLeft += async (user) =>
            {
                if (user.IsBot)
                    return;
                if (Config.Load().ExtraLogging)
                    await Logging.TryLoggingAsync($":x:  `{user.Username}#{user.Discriminator}` has left the server!");
            };

            client.MessageReceived += async (s) =>
            {
                if (Config.Load().WordFilter == true)
                {
                    if (s.Author.IsBot)
                        return;
                    if (Config.Load().IgnoredChannels.Contains(s.Channel.Id))
                        return;
                    var words = Config.Load().Words;
                    if (words.Any(s.Content.ToLower().Contains))
                    {
                        var log = client.GetChannel(Config.Load().LogChannel) as SocketTextChannel;
                        if (log == null)
                            await s.DeleteAsync();
                        else
                            await log.SendMessageAsync($":anger: {s.Author.Mention} violated the word filter. **Message Deleted**");
                        await s.DeleteAsync();
                    }
                }
            };

            client.MessageDeleted += async (message, channel) =>
            {
                if (Config.Load().ExtraLogging)
                {
                    if (!message.HasValue)
                        return;
                    var msg = await message.GetOrDownloadAsync();
                    if (msg.Content.Contains("~clean"))
                        return;
                    if (msg.Author.IsBot)
                        return;
                    if (Config.Load().IgnoredChannels.Contains(channel.Id))
                        return;
                    if (!msg.Attachments.Any())
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: (`#{channel.Name}`) **{msg.Author.Username}** deleted their message:\n{msg.Content}");
                    else
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: (`#{channel.Name}`) **{msg.Author.Username}** deleted their message:\n{msg.Content}\n{msg.Attachments.FirstOrDefault().Url}");
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
                        await Logging.TryLoggingAsync($":heavy_plus_sign: **{after.Author.Username}** edited their message:\n**Before:** {old.Content}\n**After:** {after.Content}");
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
        }
    }
}