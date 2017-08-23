using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XDB.Common.Models;
using System.IO;

namespace XDB.Services
{
    public class ModerationService
    {
        public List<TempBan> FetchTempBans()
            => JsonConvert.DeserializeObject<List<TempBan>>(File.ReadAllText(Xeno.TempBanPath));

        public IEnumerable<TempBan> FetchActiveTempBans()
            => FetchTempBans().Where(x => DateTime.Compare(DateTime.UtcNow, x.UnbanTime) > 0);

        public List<Mute> FetchMutes()
            => JsonConvert.DeserializeObject<List<Mute>>(File.ReadAllText(Xeno.MutesPath));

        public IEnumerable<Mute> FetchActiveMutes()
            => FetchMutes().Where(x => DateTime.Compare(DateTime.UtcNow, x.UnmuteTime) > 0 && x.IsActive);

        public void Initialize()
        {
            if(!File.Exists(Xeno.TempBanPath))
            {
                List<TempBan> bans = new List<TempBan>();
                using (var file = new FileStream(Xeno.TempBanPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.TempBanPath, JsonConvert.SerializeObject(bans));
            }

            if (!File.Exists(Xeno.MutesPath))
            {
                List<Mute> mutes = new List<Mute>();
                using (var file = new FileStream(Xeno.MutesPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.MutesPath, JsonConvert.SerializeObject(mutes));
            }
        }

        public void AddMute(Mute mute)
        {
            var _in = FetchMutes();
            try
            {
                _in.Add(mute);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.MutesPath, FileMode.Truncate))
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

        public void RemoveMute(Mute _mute)
        {
            var _in = FetchMutes();
            try
            {
                var mute = _in.Find(x => x.UserId == _mute.UserId);
                _in.Remove(mute);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.MutesPath, FileMode.Truncate))
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

        public void AddTemporaryBan(TempBan ban)
        {
            var _in = FetchTempBans();
            try
            {
                _in.Add(ban);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.TempBanPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Temporary Bans", e.ToString());
            }
        }

        public void RemoveTemporaryBan(TempBan _ban)
        {
            var _in = FetchTempBans();
            try
            {
                var ban = _in.Find(x => x.Timestamp == _ban.Timestamp);
                _in.Remove(ban);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.TempBanPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Temporary Bans", e.ToString());
            }
        }
    }
}
