﻿using Discord;
using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Modules
{
    [Summary("Utility")]
    [RequireContext(ContextType.Guild)]
    public class Utility : ModuleBase<SocketCommandContext>
    {
        private CustomsearchService _search;

        protected override void BeforeExecute(CommandInfo command)
        {
            if (string.IsNullOrEmpty(Config.Load().GoogleKey))
                throw new InvalidOperationException("Google API Key is not set, Google Query failed.");

            _search = new CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey = Config.Load().GoogleKey,
                MaxUrlLength = 256
            });
        }

        [Command("google"), Alias("g"), Summary("Returns a search query from google.")]
        public async Task Google([Remainder] string query)
        {
            var results = (await SearchGoogleAsync(query)).ToArray();
            var embed = new EmbedBuilder().WithColor(new Color(16, 178, 232)).WithTitle($"Search results for: {query}").WithDescription($"{results[0].Link}\n\nSee also:\n{results[1].Link}\n{results[2].Link}");
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("urban"), Summary("Retrieves a definition from UrbanDictionary.")]
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
                    await ReplyAsync(":heavy_multiplication_x:  Could not find a definition for that word!");
                }
            }
        }

        private async Task<IEnumerable<Result>> SearchGoogleAsync(string query, Uri site = null)
        {
            var request = _search.Cse.List(query);
            request.Cx = "010537608855081241185:b3t-ivrqjwg";
            request.Num = 3;
            request.Safe = CseResource.ListRequest.SafeEnum.Medium;

            if (site != null)
                request.SiteSearch = site.ToString();

            var result = await request.ExecuteAsync();
            return result.Items;
        }
    }
}
