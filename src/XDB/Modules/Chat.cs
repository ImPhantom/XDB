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
        [Command("info")]
        [Remarks("Displays the bots information and statistics.")]
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
                x.Value = $"{application.Owner.Mention} (**ID:** `{application.Owner.Id}`)";
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
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Heap Size:";
                x.Value = $"`{GetHeapSize()}MB`";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Guilds/Channels/Users:";
                x.Value = $"{(Context.Client as DiscordSocketClient).Guilds.Count} / {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)} / {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}";
                x.IsInline = true;
            });

            await ReplyAsync("", false, embed.Build());
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
