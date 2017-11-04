using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Xml;

namespace XDB
{
    public class Xeno
    {
        public static readonly string Version = "1.3.2";
        public static string Status = $"XDB (rel: {Version})(api: {DiscordConfig.Version})";

        // Default Messages
        public static string[] EightBallResponses = { "It is certain", "It is decidedly so", "Without a doubt", "Yes, definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes", "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful" };

        public static string ConfigCreated = @"After you input your token, a config will be generated at 'cfg\\config.json'.
Please fill in all your info and restart the bot.";
        public static string LoggerFailed = "[Logging] [Error] Log message failed to send! Either set your Logging Channel ID in cfg/config.json or use the '~log <channel-id>' command.";
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

        #region paths
        public static string ModulePath = Path.Combine(AppContext.BaseDirectory, "cfg/modules.json");
        public static string TodoPath = Path.Combine(AppContext.BaseDirectory, $"data/todolists.json");
        public static string DevListPath = Path.Combine(AppContext.BaseDirectory, $"data/channel_lists.json");
        public static string TagsPath = Path.Combine(AppContext.BaseDirectory, $"data/tags.json");
        public static string WarnPath = Path.Combine(AppContext.BaseDirectory, $"data/warns.json");
        public static string MutesPath = Path.Combine(AppContext.BaseDirectory, $"data/mutes.json");
        public static string RemindPath = Path.Combine(AppContext.BaseDirectory, $"data/reminders.json");
        public static string TempBanPath = Path.Combine(AppContext.BaseDirectory, $"data/tempbans.json");
        public static string CringePath = Path.Combine(AppContext.BaseDirectory, $"data/boardmessages.json");

        public static string CachePath = Path.Combine(AppContext.BaseDirectory, $"data/audio_cache");
        #endregion

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
            if (!user.Game.HasValue)
                return $"`N/A`";
            else
                return $"`{user.Game}`";
        }

        public static int ParseDuration(string duration)
        {
            if(duration.StartsWith("PT"))
            {
                var span = XmlConvert.ToTimeSpan(duration);
                var parsed = Convert.ToInt32(span.TotalSeconds);
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
    }
}
