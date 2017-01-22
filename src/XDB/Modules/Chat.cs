using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Modules
{
    public class Chat : ModuleBase
    {
        [Command("userinfo")]
        [Remarks("Display's your information.")]
        [RequireContext(ContextType.Guild)]
        public async Task UserInfo()
        {
            var date = $"{Context.Guild.CreatedAt.Month}/{Context.Guild.CreatedAt.Day}/{Context.Guild.CreatedAt.Year}";
            var user = new EmbedAuthorBuilder()
            {
                Name = Context.User.Username,
                IconUrl = Context.User.AvatarUrl
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Author = user
            };
            embed.AddField(x =>
            {
                x.Name = "Username:";
                x.Value = $"{Context.User.Username}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Discriminator:";
                x.Value = $"{Context.User.Discriminator}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "ID:";
                x.Value = $"`{Context.User.Id}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Created:";
                x.Value = date;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Status:";
                x.Value = $"{Context.User.Status}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Game:";
                x.Value = $"{GetUserGame(Context)}";
                x.IsInline = true;
            });
            await ReplyAsync("", false, embed.Build());
        }

        [Command("serverinfo")]
        [Remarks("Display's the current guild's information.")]
        [RequireContext(ContextType.Guild)]
        public async Task ServerInfo()
        {
            // Ignore this shit
            var owner = await Context.Guild.GetOwnerAsync();
            var users = await Context.Guild.GetUsersAsync();
            var text = await Context.Guild.GetTextChannelsAsync();
            var voice = await Context.Guild.GetVoiceChannelsAsync();

            var date = $"{Context.Guild.CreatedAt.Month}/{Context.Guild.CreatedAt.Day}/{Context.Guild.CreatedAt.Year}";
            var footer = new EmbedFooterBuilder()
            {
                IconUrl = Context.Guild.IconUrl,
                Text = $"Server ID: {Context.Guild.Id}"
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                ThumbnailUrl = Context.Guild.IconUrl,
                Footer = footer
            };
            embed.AddField(x =>
            {
                x.Name = "Owner:";
                x.Value = owner.Mention;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Server Name:";
                x.Value = $"{Context.Guild.Name}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Voice Region:";
                x.Value = $"`{Context.Guild.VoiceRegionId}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Created:";
                x.Value = date;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Roles:";
                x.Value = $"{Context.Guild.Roles.Count}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Users:";
                x.Value = $"{users.Count}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Text Channels:";
                x.Value = $"{text.Count()}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Voice Channels:";
                x.Value = $"{voice.Count()}";
                x.IsInline = true;
            });
            await ReplyAsync("", false, embed.Build());
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

        private string GetUserGame(CommandContext ctx)
        {
            var playing = ctx.User.Game;
            if(!playing.HasValue)
            {
                return $"`n/a`";
            }
            return $"`{playing}`";
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
