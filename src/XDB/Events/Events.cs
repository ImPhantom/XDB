using Discord.WebSocket;
using System.Linq;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB.Events
{
    public class Events
    {
        public static void initEvents()
        {
            var client = Program.client;

            client.UserJoined += async (s) =>
            {
                if(Config.Load().Welcome == true)
                {
                    if (s.IsBot)
                        return;
                    var def = s.Guild.DefaultChannel;
                    await def.SendMessageAsync(s.Mention + $" {Config.Load().WelcomeMessage}");
                }
            };

            client.MessageReceived += async (s) =>
            {
                if(Config.Load().WordFilter == true)
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
                if(Config.Load().MessageLogging)
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
                    if(!msg.Attachments.Any())
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: **{msg.Author.Username}** deleted their message:\n{msg.Content}");
                    else
                        await Logging.TryLoggingAsync($":heavy_multiplication_x: **{msg.Author.Username}** deleted their message:\n{msg.Content}\n{msg.Attachments.FirstOrDefault().Url}");
                }
            };

            client.MessageUpdated += async (before, after, channel) =>
            {
                if(Config.Load().MessageLogging)
                {
                    var old = await before.GetOrDownloadAsync();
                    if (after.Author.IsBot)
                        return;
                    if (Config.Load().IgnoredChannels.Contains(channel.Id))
                        return;
                    await Logging.TryLoggingAsync($":heavy_plus_sign: **{after.Author.Username}** edited their message:\n**Before:** {old.Content}\n**After:** {after.Content}");
                }
            };
        }
    }
}
