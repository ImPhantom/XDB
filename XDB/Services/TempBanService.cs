using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XDB.Common.Models;
using XDB.Common.Types;

namespace XDB.Services
{
    public class TempBanService
    {
        public static List<TempBan> FetchBans()
            => JsonConvert.DeserializeObject<List<TempBan>>(File.ReadAllText(Xeno.TempBanPath));

        public IEnumerable<TempBan> GetActiveBans()
            => FetchBans().Where(x => DateTime.Compare(DateTime.UtcNow, x.UnbanTime) > 0);

        public void Initialize()
        {
            if (File.Exists(Xeno.TempBanPath))
                return;
            else
            {
                List<TempBan> bans = new List<TempBan>();
                var json = JsonConvert.SerializeObject(bans);
                using (var file = new FileStream(Xeno.TempBanPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.TempBanPath, json);
            }
        }

        public static void AddTemporaryBan(TempBan ban)
        {
            var _in = FetchBans();
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

        public static void RemoveTemporaryBan(TempBan _ban)
        {
            var _in = FetchBans();
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
