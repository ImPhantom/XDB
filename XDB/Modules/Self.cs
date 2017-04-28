﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Self")]
    public class Self : ModuleBase<SocketCommandContext>
    {
        [Command("changelog"), Summary("Displays the XDB Changelog for this version.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Changelog()
        {
            var log = $"I've been updated to `v{Strings.ReleaseVersion}`\n\n**Changes:**\n~ Cleaned up todo/warn/rep\n~ Improved welcome/info modules\n~ Much needed asthetic changes";
            await ReplyAsync(log);
        }

        [Command("nick"), Summary("Sets the bots nickname.")]
        [Name("nick `<string>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task SetNick([Remainder] string str)
        {
            var current = Context.Guild.CurrentUser;
            await current.ModifyAsync(x => x.Nickname = str);
            await ReplyAsync(":heavy_check_mark:  You set the bots nickname to: `" + str + "`");
        }

        [Command("status"), Summary("Sets the bots status.")]
        [Name("status `<string>`")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetGame([Remainder] string str)
        {
            await (Context.Client as DiscordSocketClient).SetGameAsync(str);
            await ReplyAsync(":heavy_check_mark:  You set the bots status to: `" + str + "`");
        }

        [Command("pres"), Summary("Sets the bots presence.")]
        [Name("pres (`online`, `idle`, `dnd`, `invis`)")]
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
        [Name("avatar `<url>`")]
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
        [Name("username `<string>`")]
        [Permissions(AccessLevel.BotOwner)]
        public async Task SetUsername([Remainder] string str)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = str);
            await ReplyAsync($":heavy_check_mark:  You set the bots username to: `{str}`");
        }

        [Command("leave"), Summary("Forces the bot to leave its current guild.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Leave()
            => await Context.Guild.LeaveAsync();

        [Command("logchannel"), Summary("Sets the logging channel.")]
        [Alias("log")]
        [Name("log `<channel-id>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task LogChannel(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.LogChannel == channelid) { await ReplyAsync($":black_medium_small_square:  That already is the logging channel."); return; }
            cfg.LogChannel = channelid;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You set the logging channel to: `{channelid.ToString()}`");
        }

        [Command("filters"), Summary("Displays all words in the word filter.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterWords()
        {
            var words = new StringBuilder();
            if (!Config.Load().Words.Any()) { await ReplyAsync(":black_medium_small_square:  There are no words in the word filter."); return; }
            foreach(var word in Config.Load().Words)
            {
                words.AppendLine(word);
            }
            await ReplyAsync($":heavy_check_mark:  Currently Blacklisted Words:\n```\n{words.ToString()}\n```");
        }

        [Command("addword"), Summary("Adds a word/string to the word filter.")]
        [Name("addword `<string>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterAdd([Remainder] string str)
        {
            var cfg = Config.Load();
            if (cfg.Words.Contains(str)) { await ReplyAsync($":black_medium_small_square:  There is already a string matching `{str}` in the word filter."); return; }
            cfg.Words.Add(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You added `{str}` to the word filter!");
        }

        [Command("delword"), Summary("Removes a word/string from the word filter.")]
        [Name("delword `<string>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterDel([Remainder] string str)
        {
            var cfg = Config.Load();
            if (!cfg.Words.Contains(str)) { await ReplyAsync($":black_medium_small_square:  There is no string matching `{str}` in the word filter."); return; }
            cfg.Words.Remove(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_multiplication_x:  You removed `{str}` from the word filter!");
        }

        [Command("ignore"), Summary("Adds a channel to the list of ignored channels.")]
        [Name("ignore `<channel-id>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ignore(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":black_medium_small_square:  That channel is already ignored."); return; }
            cfg.IgnoredChannels.Add(channelid);
            cfg.Save();
            var channel = Context.Guild.GetTextChannel(channelid);
            await Logging.TryLoggingAsync($":heavy_check_mark:  **{Context.User.Username}#{Context.User.Discriminator}** added {channel.Mention} (`{channelid}`) to the ignored channels list.");
        }

        [Command("ignored"), Summary("Displays all ignored channels.")]
        [Name("ignored")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ignored()
        {
            var ignored = new StringBuilder();
            if (!Config.Load().IgnoredChannels.Any()) { await ReplyAsync(":black_medium_small_square:  There are no ignored channels!"); return; }
            foreach(var channel in Config.Load().IgnoredChannels)
            {
                var chan = Context.Guild.GetTextChannel(channel);
                ignored.AppendLine($"**#{chan.Name}** -- `{chan.Id}`");
            }
            await ReplyAsync($":heavy_check_mark:  Currently ignored channels:\n{ignored.ToString()}");
        }

        [Command("delignore"), Summary("Removes a channel from the list of ignored channels.")]
        [Name("delignore `<channel-id>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelIgnore(ulong channelid)
        {
            var cfg = Config.Load();
            if(!cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":black_medium_small_square:  That channel is not ignored."); return; }
            cfg.IgnoredChannels.Remove(channelid);
            cfg.Save();
            var channel = Context.Guild.GetTextChannel(channelid);
            await Logging.TryLoggingAsync($":heavy_multiplication_x:  **{Context.User.Username}#{Context.User.Discriminator}** removed {channel.Mention} (`{channelid}`) from the ignored channels list.");
        }

        [Command("welcome"), Summary("Toggles the welcome message.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Welcome()
        {
            var cfg = Config.Load();
            if (cfg.Welcome == true)
            {
                cfg.Welcome = false;
                await ReplyAsync($":heavy_multiplication_x:  Welcome Message disabled.");
            } else
            {
                cfg.Welcome = true;
                await ReplyAsync($":heavy_check_mark:  Welcome Message enabled.");
            }
            cfg.Save();
        }

        [Command("welcome"), Summary("Sets the welcome message.")]
        [Name("welcome `<message>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Welcome([Remainder] string message)
        {
            var cfg = Config.Load();
            cfg.WelcomeMessage = message;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You changed the welcome message to: \n\n{message}");
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
