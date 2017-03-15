using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XDB.Modules
{
    [Summary("Utility")]
    [RequireContext(ContextType.Guild)]
    public class Utility : ModuleBase
    {
        [Command("shorten"), Summary("Shortens a link.")]
        [Name("shorten `<url>`")]
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
                await ReplyAsync($"**Error:** Shortener is down.\n**Exception: **{ex.Message}");
            }
        }

        [Command("urban"), Summary("Retrieves a definition from UrbanDictionary.")]
        [Name("urban `<string>`")]
        public async Task Urban([Remainder]string define)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var get = await client.GetStringAsync($"http://api.urbandictionary.com/v0/define?term={Uri.EscapeUriString(define)}");
                try
                {
                    var items = JObject.Parse(get);
                    var item = items["list"][0];
                    var word = item["word"].ToString();
                    var def = item["definition"].ToString();
                    var link = item["permalink"].ToString();

                    var embed = new EmbedBuilder() { Color = new Color(21, 144, 232) };
                    embed.AddField(x =>
                    {
                        x.Name = $"{word}";
                        x.Value = $"\"{def}\"\n\nPermalink: {link}";
                        x.IsInline = false;
                    });
                    await ReplyAsync("", false, embed.Build());
                } catch
                {
                    await ReplyAsync(":anger: Could not find a definition for that word!");
                }
            }
        }
    }
}
