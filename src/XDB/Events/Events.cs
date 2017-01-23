using Discord;
using System.Linq;
using XDB.Common.Types;

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
                    var def = await s.Guild.GetDefaultChannelAsync();
                    await def.SendMessageAsync(s.Mention + $" {Config.Load().WelcomeMessage}");
                }
            };

            client.MessageReceived += async (s) =>
            {
                //Make word filter better
                if(Config.Load().WordFilter == true)
                {
                    var words = Config.Load().Words;
                    if (words.Any(s.Content.Contains))
                    {
                        var log = client.GetChannel(Config.Load().LogChannel) as ITextChannel;
                        if (log == null)
                            await s.DeleteAsync();
                        else
                            await log.SendMessageAsync($":anger: {s.Author.Mention} violated the word filter. **Message Deleted**");
                            await s.DeleteAsync();

                        //If in log channel ignore (deleted message in logging)
                    }
                }
            };
        }
    }
}
