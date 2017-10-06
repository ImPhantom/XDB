using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XDB.Common.Models;

namespace XDB.Services
{
    public class RemindService
    {
        public List<Reminder> FetchReminders()
            => JsonConvert.DeserializeObject<List<Reminder>>(File.ReadAllText(Xeno.RemindPath));

        public IEnumerable<Reminder> FetchActiveReminders()
            => FetchReminders().Where(x => DateTime.Compare(DateTime.UtcNow, x.RemindTime) > 0);

        public void AddReminder(Reminder reminder)
        {
            var _in = FetchReminders();
            try
            {
                _in.Add(reminder);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.RemindPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Remind", e.ToString());
            }
        }

        public void RemoveReminder(Reminder _reminder)
        {
            var _in = FetchReminders();
            try
            {
                var reminder = _in.Find(x => x.Timestamp == _reminder.Timestamp);
                _in.Remove(reminder);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.RemindPath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Remind", e.ToString());
            }
        }

        public void Initialize()
        {
            if (!File.Exists(Xeno.RemindPath))
            {
                List<Reminder> reminders = new List<Reminder>();
                var json = JsonConvert.SerializeObject(reminders);
                using (var file = new FileStream(Xeno.RemindPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.RemindPath, json);
            }
        }
    }
}
