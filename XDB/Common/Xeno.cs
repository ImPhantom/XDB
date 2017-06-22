using Discord;
using Discord.WebSocket;
using System;
using System.IO;

namespace XDB.Common.Types
{
    public class Xeno
    {
        public static readonly string Version = "1.2.7rc1";
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
        public static string ConfigPath = Path.Combine(AppContext.BaseDirectory, "cfg/config.json");
        public static string ModulePath = Path.Combine(AppContext.BaseDirectory, "cfg/modules.json");
        public static string TodoPath = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
        public static string TagsPath = Path.Combine(AppContext.BaseDirectory, $"tags/tags.json");
        public static string RepPath = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
        public static string WarnPath = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
        //public static string RatePath = Path.Combine(AppContext.BaseDirectory, $"cfg/ratelimit.json");
        public static string MutesPath = Path.Combine(AppContext.BaseDirectory, $"mutes.json");
        public static string RemindPath = Path.Combine(AppContext.BaseDirectory, $"reminders.json");
        public static string TempBanPath = Path.Combine(AppContext.BaseDirectory, $"tempbans.json");
        public static string CringePath = Path.Combine(AppContext.BaseDirectory, $"boardmessages.json");
        #endregion

        public static string GetUserGame(SocketGuildUser user)
        {
            if (!user.Game.HasValue)
                return $"`N/A`";
            else
                return $"`{user.Game}`";
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
