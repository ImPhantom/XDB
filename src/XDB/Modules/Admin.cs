using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;

namespace XDB.Modules
{
    public class Admin : ModuleBase
    {
        [Command("cleanup")]
        [Remarks("Cleans up specified amount of messages from channel.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Cleanup(int amt)
        {
            if (amt < 1)
                return;
            amt += 1;
            await Context.Message.DeleteAsync().ConfigureAwait(false);
            int lim = (amt < 100) ? amt : 100;
            var messages = (await Context.Channel.GetMessagesAsync().Flatten().ConfigureAwait(false));
            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("kick")]
        [Remarks("Kicks user from guild.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Kick(IGuildUser user, [Remainder] string str)
        {
            try
            {
                await Context.Channel.SendMessageAsync($":grey_exclamation: {Context.Message.Author.Mention} has kicked {user.Mention} from {Context.Guild.Name} (Reason: `{str}`)");
                await user.KickAsync().ConfigureAwait(false);
            }
            catch { }
        }
    }
}
