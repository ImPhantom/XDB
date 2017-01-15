﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Common.Types
{
    public class Config
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;


        public string Prefix { get; set; }
        public ulong[] Owners { get; set; }
        public string Token { get; set; }

        public Config()
        {
            Prefix = "~";
            Owners = new ulong[] { 0 };
            Token = "";
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
    }
}
