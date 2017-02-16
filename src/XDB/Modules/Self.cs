using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Common.Types;

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

        [Command("leave")]
        [Remarks("Makes the bot leave the server.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Leave()
        {
            var current = await Context.Guild.GetCurrentUserAsync();
            await current.Guild.LeaveAsync();
        }

        [Command("info")]
        [Remarks("Display's the bots information and statistics.")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var auth = new EmbedAuthorBuilder()
            {
                Name = Context.Client.CurrentUser.Username,
                IconUrl = Context.Client.CurrentUser.AvatarUrl
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Author = auth
            };
            embed.AddField(x =>
            {
                x.Name = "Author:";
                x.Value = $"{application.Owner.Username} (**ID:** `{application.Owner.Id}`)";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Version:";
                x.Value = $"`XDB v{Strings.ReleaseVersion}`";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Library:";
                x.Value = $"Discord.Net (`{DiscordConfig.Version}`)";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Runtime:";
                x.Value = $"{RuntimeInformation.FrameworkDescription} ({RuntimeInformation.OSArchitecture})";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Host OS:";
                x.Value = RuntimeInformation.OSDescription;
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Uptime:";
                x.Value = $"`{GetUptime()}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Heap Size:";
                x.Value = $"`{GetHeapSize()}MB`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Guilds/Channels/Users:";
                x.Value = $"{(Context.Client as DiscordSocketClient).Guilds.Count} / {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)} / {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}";
                x.IsInline = false;
            });

            await ReplyAsync("", false, embed.Build());
        }


        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
