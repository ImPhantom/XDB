﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using XDB.Common.Attributes;

namespace XDB.Common.Types
{
    public class Config
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string Prefix { get; set; }
        public ulong[] Owners { get; set; }
        public string Token { get; set; }
        public ulong LogChannel { get; set; }
        public ulong[] IgnoredChannels { get; set; }
        public bool WordFilter { get; set; }
        public string[] Words { get; set; }
        public bool Welcome { get; set; }
        public string WelcomeMessage { get; set; }

        public Config()
        {
            Prefix = "~";
            Owners = new ulong[] { 0 };
            Token = "";
            LogChannel = 0;
            IgnoredChannels = new ulong[] { 0 };
            WordFilter = false;
            Words = new string[] { "fuck", "shit" };
            Welcome = false;
            WelcomeMessage = "**Welcome to the server!**";
        }

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
                List<string> item = new List<string>();
                item.Add("Default");
                lists.Add(new UserTodo()
                {
                    Id = 0,
                    ListItems = item
                });
                var json = JsonConvert.SerializeObject(lists);
                using (var file = new FileStream(path, FileMode.Create)) { }
                File.WriteAllText(path, json);
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
                reps.Add(new UserRep()
                {
                    Id = 0,
                    Rep = 0
                });
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
                List<string> defWarns = new List<string> { "Default" };
                warns.Add(new UserWarn()
                {
                    WarnedUser = 0,
                    WarnReason = defWarns
                });
                var json = JsonConvert.SerializeObject(warns);
                using (var file = new FileStream(path, FileMode.Create)) { }
                File.WriteAllText(path, json);
            }
            else
                return;
        }
    }
}
