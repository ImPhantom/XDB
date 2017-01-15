using Discord;
using Discord.Commands;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace XDB.Modules
{
    public class Self : ModuleBase
    {
        [Command("bstat")]
        [Alias("status")]
        [Remarks("Sets the bots status.")]
        [RequireOwner]
        public async Task SetGame([Remainder] string str)
        {
            await Program.client.SetGameAsync(str);
            await ReplyAsync(":grey_exclamation: You set the bots status to: `" + str + "`");
        }

        [Command("bavatar")]
        [Alias("avatar")]
        [Remarks("Sets the bots avatar.")]
        [RequireOwner]
        public async Task SetAvatar([Remainder] string str)
        {
            using (var http = new HttpClient())
            {
                using (var response = await http.GetStreamAsync(str))
                {
                    var imgStream = new MemoryStream();
                    await response.CopyToAsync(imgStream);
                    imgStream.Position = 0;

                    await Program.client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(imgStream));
                    await ReplyAsync(":grey_exclamation: You set the bots avatar!");
                }
            }
        }

        [Command("bnick")]
        [Alias("nick")]
        [Remarks("Sets the bots nickname.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetNick([Remainder] string str)
        {
            var bot = await Context.Guild.GetCurrentUserAsync();
            await bot.ModifyAsync(x => x.Nickname = str);
            await ReplyAsync(":grey_exclamation: You set the bots nickname to: `" + str + "`");
        }

        [Command("kick")]
        [Remarks("Kicks user from guild.")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
