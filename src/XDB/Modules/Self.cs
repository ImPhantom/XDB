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

        [Command("logchannel")]
        [Alias("log")]
        [Name("log `<channel-id>`")]
        [Remarks("Sets the logging channel.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task LogChannel(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.LogChannel == channelid) { await ReplyAsync($":anger: That is already the logging channel."); return; }
            cfg.LogChannel = channelid;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You set the logging channel to: `{channelid.ToString()}`");
        }

        [Command("filters")]
        [Remarks("Displays all words in the word filter.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterWords()
        {
            var words = new StringBuilder();
            if (!Config.Load().Words.Any()) { await ReplyAsync(":anger: There are no words in the word filter."); return; }
            foreach(var word in Config.Load().Words)
            {
                words.AppendLine(word);
            }
            await ReplyAsync($":heavy_check_mark:  Currently Blacklisted Words:\n```\n{words.ToString()}\n```");
        }

        [Command("addword")]
        [Name("addword `<string>`")]
        [Remarks("Adds a word/string to the word filter.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterAdd([Remainder] string str)
        {
            var cfg = Config.Load();
            if (cfg.Words.Contains(str)) { await ReplyAsync($":anger: There is already a string matching `{str}` in the word filter."); return; }
            cfg.Words.Add(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You added `{str}` to the word filter!");
        }

        [Command("delword")]
        [Name("delword `<string>`")]
        [Remarks("Removes a word/string from the word filter.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task FilterDel([Remainder] string str)
        {
            var cfg = Config.Load();
            if (!cfg.Words.Contains(str)) { await ReplyAsync($":anger: There is no string matching `{str}` in the word filter."); return; }
            cfg.Words.Remove(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_multiplication_x:  You removed `{str}` from the word filter!");
        }

        [Command("ignore")]
        [Name("ignore `<channel-id>`")]
        [Remarks("Adds a channel to the ignored channels.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ignore(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":anger: That channel is already ignored."); return; }
            cfg.IgnoredChannels.Add(channelid);
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You added channel `{channelid}` to the ignored channels list.");
        }

        [Command("ignored")]
        [Name("ignored")]
        [Remarks("Displays the ignored channels.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ignored()
        {
            var ignored = new StringBuilder();
            if (!Config.Load().IgnoredChannels.Any()) { await ReplyAsync(":anger: There are no ignored channels!"); return; }
            foreach(var channel in Config.Load().IgnoredChannels)
            {
                var chan = await Context.Guild.GetChannelAsync(channel);
                ignored.AppendLine($"**#{chan.Name}** -- `{chan.Id}`");
            }
            await ReplyAsync($":heavy_check_mark:  Currently ignored channels:\n{ignored.ToString()}");
        }

        [Command("delignore")]
        [Name("delignore `<channel-id>`")]
        [Remarks("Removes a channel from the ignored channels.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelIgnore(ulong channelid)
        {
            var cfg = Config.Load();
            if(!cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":anger: That channel is not ignored."); return; }
            cfg.IgnoredChannels.Remove(channelid);
            cfg.Save();
            await ReplyAsync($":heavy_multiplication_x:  You removed channel `{channelid}` from the ignored channels list.");
        }

        [Command("welcome")]
        [Remarks("Toggles the welcome message.")]
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

        [Command("welcome")]
        [Name("welcome `<message>`")]
        [Remarks("Toggles the welcome message.")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Welcome([Remainder] string message)
        {
            var cfg = Config.Load();
            cfg.WelcomeMessage = message;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You changed the welcome message to: \n\n{message}");
        }

        // Need to find a better way to do this...
        /*[Command("suggest")]
        [Name("suggest `<idea>`")]
        [Remarks("Suggests a feature to be added to XDB.")]
        public async Task Suggest([Remainder] string str)
        {
            var dev = await Context.Client.GetUserAsync(93765631177920512);
            var dm = await dev.CreateDMChannelAsync();
            await dm.SendMessageAsync($"**{Context.User.Username}** submitted a suggestion:\n\n`{str}`");
            await ReplyAsync(":white_check_mark: You submitted your suggestion!");
        }*/

        #region info (embed)
        [Command("info")]
        [Remarks("Display's the bots information and statistics.")]
        public async Task Info()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var avurl = Context.Client.CurrentUser.GetAvatarUrl();
            var auth = new EmbedAuthorBuilder()
            {
                Name = Context.Client.CurrentUser.Username,
                IconUrl = avurl
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
        #endregion
    }
}
