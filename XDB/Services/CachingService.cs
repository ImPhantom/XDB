using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Common.Types;

namespace XDB.Services
{
    public class CachingService
    {
        public List<CachedVideo> CachedVideos = new List<CachedVideo>();

        public async Task Initialize()
        {
            if (!Directory.Exists(Xeno.CachePath))
                Directory.CreateDirectory(Xeno.CachePath);
            await ClearCache();
        }

        public async Task<CachedVideo> CacheVideo(string url)
        {
            int duration;
            if (url.Contains("ytsearch"))
                duration = GetDuration(url);
            else
                duration = await GetDurationAsync(url);

            if (duration == 0)
                return null;

            if (duration < Config.Load().AudioDurationLimit)
            {
                var prc = CreateProcess(url);
                var json = await prc.StandardOutput.ReadToEndAsync();
                prc.WaitForExit();
                var output = JsonConvert.DeserializeObject<CachedVideo>(json);
                CachedVideos.Add(output);
                return output;
            }
            else
                return null;
            
        }

        private Process CreateProcess(string arg)
        {
            var filename = Guid.NewGuid();
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                FileName = "youtube-dl",
                Arguments = $"-o \"{Xeno.CachePath}/{filename}.mp3\" --extract-audio --no-overwrites --print-json --audio-format mp3 {arg}"
            });
        }

        private int GetDuration(string arg) // Takes 4500-4750ms
        {
            var proc = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                FileName = "youtube-dl",
                Arguments = $"--get-duration {arg}"
            });
            var dur = proc.StandardOutput.ReadToEnd();
            return Xeno.ParseDuration(dur);
        }

        public async Task<int> GetDurationAsync(string url) // Takes 250-350ms
        {
            try
            {
                var id = Regex.Match(url, @"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([\w-]{10,12})(?:$|\&|\?\#).*").Groups[1].Value;
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync($"https://www.googleapis.com/youtube/v3/videos?id={id}&key={Config.Load().GoogleKey}&part=contentDetails");
                    var json = JsonConvert.DeserializeObject<VideoInfo>(response);
                    if (json.Items.Count() > 0)
                        return Xeno.ParseDuration(json.Items.First().Details.Duration);
                    else // if the url couldnt be parsed by this regex
                        return GetDuration(url); // use slow ass yt-dl
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Caching", $"{e.Message} : {e.StackTrace}");
                return 0;
            }
            
        }

        public Task ClearCache()
        {
            var cache = Directory.GetFiles(Xeno.CachePath);
            foreach (var file in cache)
                File.Delete(file.ToString());
            return Task.CompletedTask;
        }

        public long GetCacheFolderSize()
        {
            return new DirectoryInfo(Xeno.CachePath).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        public int GetCachedFileCount()
        {
            return new DirectoryInfo(Xeno.CachePath).GetFiles("*.*", SearchOption.AllDirectories).Count();
        }
    }
}
