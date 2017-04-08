﻿using Discord.Commands;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using Discord;

namespace XDB.Modules
{
    [Summary("Steam")]
    [RequireContext(ContextType.Guild)]
    public class Steam : ModuleBase
    {
        [Command("query"), Summary("Querys a source server for information about itself.")]
        [Name("query `<ip:port>`")]
        public async Task Query(string info)
        {
            var ip = info.Split(':');
            if (ip.Length != 2)
                return;
            var url = $"http://xeno.nn.pe/xdb/?ip={ip[0]}&port={ip[1]}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                Stream str = await content.ReadAsStreamAsync();
                StreamReader rdr = new StreamReader(str);
                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    string vac;
                    line = Regex.Replace(line, "<br />", Environment.NewLine);
                    string[] lines = line.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    if (lines[6] == "1") { vac = "Yes"; } else { vac = "No"; } //Makes VAC Status readable 
                    var embed = new EmbedBuilder()
                    {
                        Color = new Color(29, 140, 209)
                    };
                    embed.AddField(x =>
                    {
                        x.Name = "Game:";
                        x.Value = lines[0];
                        x.IsInline = false;
                    });
                    embed.AddField(x =>
                    {
                        x.Name = "Hostname:";
                        x.Value = $"`{lines[1]}`";
                        x.IsInline = false;
                    });
                    embed.AddField(x =>
                    {
                        x.Name = "Players:";
                        x.Value = $"{lines[2]}/{lines[3]}";
                        x.IsInline = false;
                    });
                    embed.AddField(x =>
                    {
                        x.Name = "Map:";
                        x.Value = $"`{lines[4]}`";
                        x.IsInline = false;
                    });
                    embed.AddField(x =>
                    {
                        x.Name = "Gamemode:";
                        x.Value = $"{lines[5]}";
                        x.IsInline = false;
                    });
                    embed.AddField(x =>
                    {
                        x.Name = "VAC Secure:";
                        x.Value = $"{vac}";
                        x.IsInline = false;
                    });
                    await ReplyAsync("", false, embed.Build());
                }
            }
        }

        [Command("players"), Summary("Retrives the playerlist from a source server.")]
        [Name("players `<ip:port>`")]
        [RequireContext(ContextType.Guild)]
        public async Task Players(string info)
        {
            var ip = info.Split(':');
            if (ip.Length != 2)
                return;
            var url = $"http://xeno.nn.pe/xdb/players.php?ip={ip[0]}&port={ip[1]}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                Stream str = await content.ReadAsStreamAsync();
                StreamReader rdr = new StreamReader(str);
                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    line = Regex.Replace(line, "<br />", Environment.NewLine);
                    string[] lines = line.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    var embed = new EmbedBuilder()
                    {
                        Color = new Color(29, 140, 209)
                    };
                    embed.AddField(x =>
                    {
                        x.Name = $"Players ({lines[0]}/{lines[1]}):";
                        x.Value = $"{line.Replace(lines[1], null).Replace(lines[0], null)}";
                        x.IsInline = false;
                    });
                    await ReplyAsync("", false, embed.Build());
                }
            }
        }
    }
}