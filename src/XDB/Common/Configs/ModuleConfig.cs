using Newtonsoft.Json;
using System;
using System.IO;

namespace XDB.Common.Types
{
    public class ModuleConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public bool ChatModule { get; set; }
        public bool AdminModule { get; set; }
        public bool MathModule { get; set; }
        public bool UtilModule { get; set; }
        public bool WarnModule { get; set; }
        public bool RepModule { get; set; }
        public bool TodoModule { get; set; }
        public bool SteamModule { get; set; }
        public bool RemindModule { get; set; }

        public ModuleConfig()
        {
            ChatModule = true;
            AdminModule = true;
            MathModule = true;
            UtilModule = true;
            WarnModule = true;
            RepModule = true;
            TodoModule = true;
            SteamModule = true;
            RemindModule = true;
        }

        public static ModuleConfig Load(string dir = "cfg/modules.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<ModuleConfig>(File.ReadAllText(file));
        }

        public void Save(string dir = "cfg/modules.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
