using Discord;
using System;
using System.IO;

namespace XDB.Common.Types
{
    public class Strings
    {
        public static readonly string ReleaseVersion = "1.1.6";

        // Common Strings
        public static string XDB_Title = $"XDB (rel: {ReleaseVersion})(api: {DiscordConfig.Version})";
        public static string XDB_ConfigLoaded = "[XDB] [Info] Configuration Successfully Loaded!";
        public static string XDB_ConfigCreated = @"After you input your token, a config will be generated at 'cfg\\config.json'.
Please fill in all your info and restart the bot.";

        // Config/Storage Paths
        public static string ConfigPath = Path.Combine(AppContext.BaseDirectory, "cfg/config.json");
        public static string ModulePath = Path.Combine(AppContext.BaseDirectory, "cfg/modules.json");
        public static string TodoPath = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
        public static string RepPath = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
        public static string WarnPath = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");

        // 8-Ball Strings
        public static string[] EightBallResponses = {"It is certain", "It is decidedly so", "Without a doubt", "Yes, definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes", "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful"};

        // Console Window Header
        public static string XDB_Header = @"
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
    }
}
