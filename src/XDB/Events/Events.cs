using Discord;
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
        }
    }
}
