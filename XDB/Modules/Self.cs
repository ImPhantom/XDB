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
using XDB.Common.Types;

namespace XDB.Modules
{
    [Summary("Self")]
    public class Self : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Summary("Displays the XDB Changelog for this version.")]
        public async Task Help()
        {
            await ReplyAsync("You can find a command list here:\n https://github.com/ImPhantom/XDB/wiki/Command-List");
        }

        [Command("nick"), Summary("Sets the bots nickname.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task SetNick([Remainder] string str)
        {
            var current = Context.Guild.CurrentUser;
            await current.ModifyAsync(x => x.Nickname = str);
            await ReplyAsync(":heavy_check_mark:  You set the bots nickname to: `" + str + "`");
        }

        [Command("status"), Summary("Sets the bots status.")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetGame([Remainder] string str)
        {
            await (Context.Client as DiscordSocketClient).SetGameAsync(str);
            await ReplyAsync(":heavy_check_mark:  You set the bots status to: `" + str + "`");
        }

        [Command("pres"), Summary("Sets the bots presence.")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetStatus([Remainder] string str)
        {
            var client = Context.Client as DiscordSocketClient;
            if(str == "online")
            {
                await client.SetStatusAsync(UserStatus.Online);
                await ReplyAsync($":heavy_check_mark:  You set the bots presence to `{str}`");
            } else if(str == "idle")
            {
                await client.SetStatusAsync(UserStatus.Idle);
                await ReplyAsync($":heavy_check_mark:  You set the bots presence to `{str}`");
            } else if(str == "dnd" || str == "do not disturb")
            {
                await client.SetStatusAsync(UserStatus.DoNotDisturb);
                await ReplyAsync($":heavy_check_mark:  You set the bots presence to `{str}`");
            } else if(str == "invis" || str == "invisible")
            {
                await client.SetStatusAsync(UserStatus.Invisible);
                await ReplyAsync($":heavy_check_mark:  You set the bots presence to `{str}`");
            } else
                await ReplyAsync(":black_medium_small_square:  **Invalid presence** \n(`online`, `idle`, `do not disturb`, `invisible`)");
        }

        [Command("avatar"), Summary("Sets the bots avatar.")]
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
                    await ReplyAsync(":heavy_check_mark:  You set the bots avatar!");
                }
            }
        }

        [Command("username"), Summary("Sets the bots username.")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetUsername([Remainder] string str)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = str);
            await ReplyAsync($":heavy_check_mark:  You set the bots username to: `{str}`");
        }

        [Command("logchannel"), Summary("Sets the logging channel.")]
        [Alias("log")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task LogChannel(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.LogChannel == channelid) { await ReplyAsync($":black_medium_small_square:  That already is the logging channel."); return; }
            cfg.LogChannel = channelid;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You set the logging channel to: `{channelid.ToString()}`");
        }


        [Command("info"), Summary("Display's the bots information and statistics.")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var avatar = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder().WithName(Context.Client.CurrentUser.Username).WithIconUrl(avatar);
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(author).WithCurrentTimestamp();
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
