﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace XDB
{
    public class Xeno
    {
        public static readonly string Version = "1.3.5qol";
        public static readonly string Changelog = $@"I've updated to `v{Version}`.

**Changes:**
~ Fixed the warn argument not allowing an uncontained string.
~ Fixed some audio non-leave cases.
~ Changed how the bot replys command success for some commands.
~ Minor command formatting changes";

        public static string Status = $"XDB (rel: {Version})(api: {DiscordConfig.Version})";

        public static string[] EightBallResponses = { "It is certain", "It is decidedly so", "Without a doubt", "Yes, definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes", "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful" };

        public static string Masthead = @"
      #---------------------------------#
      #                                 #
      #       __   _______  ____        #
      #       \ \ / /  __ \|  _ \       #
      #        \ V /| |  | | |_) |      #
      #         > < | |  | |  _ <       #
      #        / . \| |__| | |_) |      #
      #       /_/ \_\_____/|____/       #
      #                                 #
      #---------------------------------#
 ";

        public static string Todo = Path.Combine(AppContext.BaseDirectory, $"data/todo.json");
        public static string TagsPath = Path.Combine(AppContext.BaseDirectory, $"data/guild_tags.json");
        public static string WarningsPath = Path.Combine(AppContext.BaseDirectory, $"data/warnings.json");
        public static string MutesPath = Path.Combine(AppContext.BaseDirectory, $"data/mutes.json");
        public static string RemindPath = Path.Combine(AppContext.BaseDirectory, $"data/reminders.json");
        public static string TempBanPath = Path.Combine(AppContext.BaseDirectory, $"data/tempbans.json");
        public static string CringePath = Path.Combine(AppContext.BaseDirectory, $"data/boardmessages.json");
        public static string BlacklistedUsersPath = Path.Combine(AppContext.BaseDirectory, $"data/blacklisted_users.json");

        public static Color RandomColor()
        {
            var rand = new Random();
            var r = rand.Next(0, 255);
            var g = rand.Next(0, 255);
            var b = rand.Next(0, 255);
            return new Color(r, g, b);
        }

        public static Embed ErrorEmbed(string error)
        {
            var embed = new EmbedBuilder().WithColor(new Color(255, 0, 0)).WithTitle("Error:").WithDescription(error);
            return embed.Build();
        }

        public static Embed InfoEmbed(string info)
        {
            var embed = new EmbedBuilder().WithColor(new Color(33, 171, 217)).WithTitle("Info").WithDescription(info);
            return embed.Build();
        }

        public static string GetUserGame(SocketGuildUser user)
        {
            if(user.Activity != null)
                return $"`{user.Activity.Name}`";
            else
                return $"`n/a`";
        }

        public static int ParseDuration(string duration)
        {
            if(duration.StartsWith("PT"))
            {
                var span = XmlConvert.ToTimeSpan(duration);
                int.TryParse(span.TotalSeconds.ToString(), out int parsed);
                return parsed;
            } else
            {
                var spl = duration.Split(':');
                int.TryParse(spl[0], out int min);
                int.TryParse(spl[1], out int sec);
                return (min * 60) + sec;
            }
            
        }

        public static string GetVoiceState(SocketGuildUser user)
        {
            var channel = user.VoiceState;
            if (user.IsMuted && user.IsDeafened || user.IsSelfMuted && user.IsSelfDeafened)
                return $"`{channel}` (Muted/Deafened)";
            else if (user.IsSelfMuted || user.IsMuted)
                return $"`{channel}` (Muted)";
            else if (user.IsSelfDeafened || user.IsDeafened)
                return $"`{channel}` (Deafened)";
            else
                return $"`{channel}`";
        }

        public static async Task SaveJsonAsync(string path, string json)
        {
            using (var stream = new FileStream(path, FileMode.Truncate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(json);
                }
            }
        }
    }
}
