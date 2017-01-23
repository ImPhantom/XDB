using Newtonsoft.Json;
using System;
using System.IO;

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
    }
}
