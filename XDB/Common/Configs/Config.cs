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

        public string Prefix { get; set; } = "~";
        public ulong[] Owners { get; set; } = new ulong[] { 0 };
        public List<ulong> Moderators { get; set; } = new List<ulong> { 0 };
        public string Token { get; set; } = "";
        public string GoogleKey { get; set; } = "";
        public ulong LogChannel { get; set; } = 0;
        public bool ExtraLogging { get; set; } = false;
        public List<ulong> IgnoredChannels { get; set; } = new List<ulong> { };
        public bool WordFilter { get; set; } = false;
        public List<string> Words { get; set; } = new List<string> { };
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

        public static void TodoCheck()
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "todo")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "todo"));

            if (!File.Exists(path))
            {
                List<UserTodo> lists = new List<UserTodo>();
                var json = JsonConvert.SerializeObject(lists);
                using (var file = new FileStream(path, FileMode.Create)) { }
                File.WriteAllText(path, json);
            }
            else
                return;
        }

        public static void TagsCheck()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "tags")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "tags"));

            if (!File.Exists(Strings.TagsPath))
            {
                List<Tag> tags = new List<Tag>();
                var json = JsonConvert.SerializeObject(tags);
                using (var file = new FileStream(Strings.TagsPath, FileMode.Create)) { }
                File.WriteAllText(Strings.TagsPath, json);
            }
            else
                return;
        }

        public static void RepCheck()
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "rep")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "rep"));

            if (!File.Exists(path))
            {
                List<UserRep> reps = new List<UserRep>();
                var json = JsonConvert.SerializeObject(reps);
                using (var file = new FileStream(path, FileMode.Create)) { }
                File.WriteAllText(path, json);
            }
            else
                return;
        }

        public static void WarnCheck()
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "warn")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "warn"));

            if (!File.Exists(path))
            {
                List<UserWarn> warns = new List<UserWarn>();
                var json = JsonConvert.SerializeObject(warns);
                using (var file = new FileStream(path, FileMode.Create)) { }
                File.WriteAllText(path, json);
            }
            else
                return;
        }

        public static void CheckExistence()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "cfg")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "cfg"));

            if (!File.Exists(Strings.ConfigPath))
            {
                var cfg = new Config();
                Console.WriteLine(Strings.XDB_ConfigCreated);
                Console.Write("Token: ");
                cfg.Token = Console.ReadLine();
                cfg.Save();
            }
            if (!File.Exists(Strings.ModulePath)) { var mdls = new ModuleConfig(); mdls.Save(); }
            BetterConsole.Log("Info", "XDB", "Configuration successfully loaded!");
        }
    }
}
