using Discord.Commands;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XDB.Modules
{
    public class Utility : ModuleBase
    {
        [Command("shorten")]
        [Name("shorten <url>")]
        [Remarks("Shortens a link.")]
        [RequireContext(ContextType.Guild)]
        public async Task Shorten([Remainder] string url)
        {
            if(!url.Contains("http"))
            {
                await ReplyAsync(":anger: Please provide a valid link.");
                return;
            }
                
            var key = "AIzaSyAVz7oHvn1PyVfY05ZnWh7BH73tS1_9EPI";
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
            try
            {
                request.ContentType = "application/json";
                request.Method = "POST";

                var requestStream = await request.GetRequestStreamAsync();
                using (var writer = new StreamWriter(requestStream))
                {
                    string json = $"{{\"longUrl\": \" {url} \"}}";
                    writer.Write(json);
                }

                var response = (HttpWebResponse)await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var shorturl = Regex.Match(reader.ReadToEnd(), @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                    await ReplyAsync($":white_check_mark: Shortened: {shorturl}");
                }
            } catch (Exception ex)
            {
                await ReplyAsync($"**Error:** Shortener is down.\n**Exception:**{ex.Message}");
            }
        }

        //TODO: Personal TODO list
    }
}
