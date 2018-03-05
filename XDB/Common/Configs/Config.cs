using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using XDB.Common.Models;

namespace XDB.Common.Types
{
    public class Config
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;
        [JsonIgnore]
        public static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "cfg/config.json");

        public string Prefix { get; set; } = "~";
        public float AudioVolume { get; set; } = .25f;
        public int AudioDurationLimit { get; set; } = 1800;
        public int PlaylistVideoLimit { get; set; } = 25;
        public List<ulong> Owners { get; set; } = new List<ulong> { };
        public ulong MutedRoleId { get; set; } = 0;
        public string Token { get; set; } = "";
        public string GoogleKey { get; set; } = "";
        public string SteamApiKey { get; set; } = "";
        public ulong LogChannel { get; set; } = 0;
        public bool BotChannelWhitelist { get; set; } = false;
        public List<ulong> WhitelistedChannels { get; set; } = new List<ulong> { };
        public bool ExtraLogging { get; set; } = false;
        public List<ulong> IgnoredChannels { get; set; } = new List<ulong> { };
        public bool Welcome { get; set; } = false;
        public string WelcomeMessage { get; set; } = "**Welcome to the server, **{mention}";

        public void Save(string dir = "cfg/config.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }

        public static Config Load(string dir = "cfg/config.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static void InitializeData()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "data"));
        }

        public static void CheckExistence()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "cfg")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "cfg"));

            if (!File.Exists(FilePath))
            {
                var cfg = new Config();
                Console.WriteLine("After you input your token, a config will be generated at 'cfg\\config.json'. \r\n Please provide your bot token and restart the bot.");
                Console.Write("Token: ");
                cfg.Token = Console.ReadLine();
                cfg.Save();
            }
            BetterConsole.Log("Info", "XDB", "Configuration successfully loaded!");
        }

        #region annoying config checks
        public static void BlacklistCheck()
        {
            if (!File.Exists(Xeno.BlacklistedUsersPath))
            {
                List<UserBlacklist> users = new List<UserBlacklist>();
                var json = JsonConvert.SerializeObject(users);
                using (var file = new FileStream(Xeno.BlacklistedUsersPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.BlacklistedUsersPath, json);
            }
        }
        #endregion
    }
}
