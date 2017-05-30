using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XDB.Common.Models;
using System.IO;
using XDB.Common.Types;

namespace XDB.Services
{
    public class MutingService
    {
        public static List<Mute> FetchMutes()
            => JsonConvert.DeserializeObject<List<Mute>>(File.ReadAllText(Strings.MutesPath));

        public IEnumerable<Mute> GetActiveMutes()
            => FetchMutes().Where(x => DateTime.Compare(DateTime.UtcNow, x.UnmuteTime) > 0 && x.IsActive);

        public void InitializeMutes()
        {
            if (File.Exists(Strings.MutesPath))
                return;
            else
            {
                List<Mute> mutes = new List<Mute>();
                var json = JsonConvert.SerializeObject(mutes);
                using (var file = new FileStream(Strings.MutesPath, FileMode.Create)) { }
                File.WriteAllText(Strings.MutesPath, json);
            }
        }

        public static void AddMute(Mute mute)
        {
            var _in = FetchMutes();
            try
            {
                _in.Add(mute);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Strings.MutesPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            } catch (Exception e)
            {
                BetterConsole.LogError("Muting", e.ToString());
            }
        }

        public static void RemoveMute(Mute _mute)
        {
            var _in = FetchMutes();
            try
            {
                var mute = _in.Find(x => x.UserId == _mute.UserId);
                _in.Remove(mute);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Strings.MutesPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Muting", e.ToString());
            }
        }
    }
}
