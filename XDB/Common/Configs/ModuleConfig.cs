using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Common.Types
{
    public class ModuleConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public bool ChatModule { get; set; }
        public bool ModerationModule { get; set; }
        public bool MathModule { get; set; }
        public bool UtilModule { get; set; }
        public bool WarnModule { get; set; }
        public bool RepModule { get; set; }
        public bool TodoModule { get; set; }
        public bool SteamModule { get; set; }
        public bool RemindModule { get; set; }
        public bool TagsModule { get; set; }

        public ModuleConfig()
        {
            ChatModule = true;
            ModerationModule = true;
            MathModule = true;
            UtilModule = true;
            WarnModule = true;
            RepModule = true;
            TodoModule = true;
            SteamModule = true;
            RemindModule = true;
            TagsModule = true;
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

        public static async Task RemoveDisabledModulesAsync(CommandService _commands)
        {
            if (!ModuleConfig.Load().ChatModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Chat")); }
            if (!ModuleConfig.Load().ModerationModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Moderation")); }
            if (!ModuleConfig.Load().MathModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Maths")); }
            if (!ModuleConfig.Load().UtilModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Utility")); }
            if (!ModuleConfig.Load().WarnModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Warn")); }
            if (!ModuleConfig.Load().RepModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Rep")); }
            if (!ModuleConfig.Load().TodoModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Todo")); }
            if (!ModuleConfig.Load().SteamModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Steam")); }
            if (!ModuleConfig.Load().RemindModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Remind")); }
            if (!ModuleConfig.Load().TagsModule) { await _commands.RemoveModuleAsync(_commands.Modules.First(x => x.Summary == "Tags")); }
        }
    }
}
