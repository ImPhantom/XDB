using System;
using System.Collections.Generic;
using System.IO;
using Discord.Commands;
using System.Threading.Tasks;
using XDB.Common.Types;
using Newtonsoft.Json;
using XDB.Common.Models;
using System.Linq;
using XDB.Common.Attributes;
using Discord;

namespace XDB.Modules
{
    [Summary("tags")]
    [RequireContext(ContextType.Guild)]
    public class Tags : ModuleBase
    {
        [Command("tag")]
        public async Task GetTag(string keyword)
        {
            Config.TagsCheck();
            var tags = File.ReadAllText(Xeno.TagsPath);
            var _json = JsonConvert.DeserializeObject<List<Tag>>(tags);
            try
            {
                var term = keyword.ToLower();
                if (!_json.Any(x => x.TagName == term))
                    await ReplyAsync(":heavy_multiplication_x:  There are no tags matching that keyword.");
                else
                {
                    var tag = _json.First(x => x.TagName == term);
                    var embed = new EmbedBuilder().WithColor(new Color(39, 217, 196)).WithDescription(tag.TagContent);
                    await ReplyAsync("", embed: embed.Build());
                }
                    
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Tags", e.ToString());
            }
        }

        [Command("tags")]
        public async Task ListTags()
        {
            Config.TagsCheck();
            var tags = File.ReadAllText(Xeno.TagsPath);
            var _json = JsonConvert.DeserializeObject<List<Tag>>(tags);
            try
            {
                if (!_json.Any())
                    await ReplyAsync(":eight_pointed_black_star:  There are no tags!");
                else
                {
                    List<string> alltags = new List<string>{};
                    _json.ForEach(x => alltags.Add(x.TagName));
                    string list = string.Join(", ", alltags);
                    await ReplyAsync($":small_blue_diamond:  **List of all tags:**```\n{list}\n```");
                }
            } catch(Exception e)
            {
                BetterConsole.LogError("Tags", e.ToString());
            }
        }

        [Command("addtag")]
        [Permissions(AccessLevel.FullAdmin)]
        public async Task AddTag(string name, [Remainder] string content)
        {
            Config.TagsCheck();
            var tags = File.ReadAllText(Xeno.TagsPath);
            var _json = JsonConvert.DeserializeObject<List<Tag>>(tags);
            try
            {
                var lower = name.ToLower();
                _json.Add(new Tag() { TagName = lower, TagContent = content });
                File.WriteAllText(Xeno.TagsPath, JsonConvert.SerializeObject(_json));
                await ReplyAsync($":heavy_check_mark:  You have added the `{lower}` tag.");
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Tags", e.ToString());
            }
        }

        [Command("removetag"), Alias("deltag", "deletetag", "remtag")]
        [Permissions(AccessLevel.FullAdmin)]
        public async Task RemoveTag(string name)
        {
            Config.TagsCheck();
            var tags = File.ReadAllText(Xeno.TagsPath);
            var _json = JsonConvert.DeserializeObject<List<Tag>>(tags);
            try
            {
                var lower = name.ToLower();
                if (!_json.Any(x => x.TagName == lower))
                    await ReplyAsync(":heavy_multiplication_x:  There are no tags matching that keyword!");
                else
                {
                    var tag = _json.First(x => x.TagName == lower);
                    _json.Remove(tag);
                    File.WriteAllText(Xeno.TagsPath, JsonConvert.SerializeObject(_json));
                    await ReplyAsync($":heavy_check_mark:  You have removed the `{lower}` tag.");
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Tags", e.ToString());
            }
        }

        [Command("edittag"), Alias("tagedit")]
        [Permissions(AccessLevel.FullAdmin)]
        public async Task EditTag(string name, [Remainder] string newtag)
        {
            Config.TagsCheck();
            var tags = File.ReadAllText(Xeno.TagsPath);
            var _json = JsonConvert.DeserializeObject<List<Tag>>(tags);
            try
            {
                var lower = name.ToLower();
                if (!_json.Any(x => x.TagName == lower))
                    await ReplyAsync(":heavy_multiplication_x:  There are no tags matching that keyword!");
                else
                {
                    var tag = _json.First(x => x.TagName == lower);
                    tag.TagContent = newtag;
                    File.WriteAllText(Xeno.TagsPath, JsonConvert.SerializeObject(_json));
                    await ReplyAsync($":heavy_check_mark:  You have successfully modified the `{lower}` tag.");
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Tags", e.ToString());
            }
        }
    }
}
