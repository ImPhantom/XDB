using Discord;
using Discord.Commands;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;

namespace XDB.Modules
{
    public class Self : ModuleBase
    {
        [Command("bnick")]
        [Name("bnick `<string>`")]
        [Alias("nick")]
        [Remarks("Sets the bots nickname.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task SetNick([Remainder] string str)
        {
            var bot = await Context.Guild.GetCurrentUserAsync();
            await bot.ModifyAsync(x => x.Nickname = str);
            await ReplyAsync(":grey_exclamation: You set the bots nickname to: `" + str + "`");
        }

        [Command("bstat")]
        [Name("bstat `<string>`")]
        [Alias("status")]
        [Remarks("Sets the bots status.")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetGame([Remainder] string str)
        {
            await Program.client.SetGameAsync(str);
            await ReplyAsync(":grey_exclamation: You set the bots status to: `" + str + "`");
        }

        [Command("bpres")]
        [Name("bpres (`online`, `idle`, `dnd`, `invis`)")]
        [Alias("pres")]
        [Remarks("Sets the bots presence.")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetStatus([Remainder] string str)
        {
            var client = Program.client;
            if(str == "online")
            {
                await client.SetStatusAsync(UserStatus.Online);
                await ReplyAsync($":grey_exclamation: You set the bots presence to `{str}`");
            } else if(str == "idle")
            {
                await client.SetStatusAsync(UserStatus.Idle);
                await ReplyAsync($":grey_exclamation: You set the bots presence to `{str}`");
            } else if(str == "dnd" || str == "do not disturb")
            {
                await client.SetStatusAsync(UserStatus.DoNotDisturb);
                await ReplyAsync($":grey_exclamation: You set the bots presence to `{str}`");
            } else if(str == "invis" || str == "invisible")
            {
                await client.SetStatusAsync(UserStatus.Invisible);
                await ReplyAsync($":grey_exclamation: You set the bots presence to `{str}`");
            } else
            {
                await ReplyAsync(":anger: **Invalid presence** \n(`online`, `idle`, `do not disturb`, `invisible`)");
            }
        }

        [Command("bavatar")]
        [Name("bavatar `<url>`")]
        [Alias("avatar")]
        [Remarks("Sets the bots avatar.")]
        [Permissions(AccessLevel.BotOwner)]
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
    }
}
