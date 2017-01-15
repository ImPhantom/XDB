using Discord;

namespace XDB.Events
{
    public class Events
    {
        public static void initEvents()
        {
            var client = Program.client;

            client.UserJoined += async (s) =>
            {
                var pub = s.Guild.GetChannel(234453502879858692) as ITextChannel;
                var ann = s.Guild.GetChannel(265984758763225088) as ITextChannel;
                await pub.SendMessageAsync($@"**Welcome to the XenoRP discord** { s.Mention }
Feel free to join a voice channel and talk to any of our staff/players.

Check { ann.Mention } for server updates/info.");
            };

            /*string[] cont = { "", "", "" };
            if (cont.Any(s.Content.Contains))
            {
                await s.Channel.SendMessageAsync("");
            }*/
        }
    }
}
