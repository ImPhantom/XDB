﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XDB.Modules
{
    public class Chat : ModuleBase
    {
        [Command("report")]
        [Remarks("Sends the report message in active channel.")]
        public async Task Report()
        {
            var reportMessage = @"If you believe that a player should be banned, gather
sufficient evidence and report them on the forums here:

  **>>>** http://xenorp.com/forumdisplay.php?fid=8 **<<<**

__The player reports are checked everyday and handled accordingly.__";
            await ReplyAsync(reportMessage);
        }

        [Command("info")]
        [Remarks("Displays the bots information and statistics.")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var auth = new EmbedAuthorBuilder()
            {
                Name = "__Xeno Discord Bot Information__"
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Author = auth
            };
            embed.AddField(x =>
            {
                x.Name = "Author:";
                x.Value = application.Owner.Mention + " (**ID:** `" + application.Owner.Id + "`)";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Library:";
                x.Value = "Discord.Net (`" + DiscordConfig.Version + "`)";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Runtime:";
                x.Value = RuntimeInformation.FrameworkDescription + RuntimeInformation.OSArchitecture;
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
                x.Value = GetHeapSize() + "MB";
                x.IsInline = false;
            });
            embed.AddField(x =>
            {
                x.Name = "Guilds:";
                x.Value = $"{(Context.Client as DiscordSocketClient).Guilds.Count}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Channels:";
                x.Value = $"{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Users:";
                x.Value = $"{(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}";
                x.IsInline = true;
            });

            await ReplyAsync("", false, embed.Build());
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
