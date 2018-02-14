using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using XDB.Common;
using XDB.Services;
using XDB.Common.Attributes;

namespace XDB.Modules
{
    [Summary("tags")]
    [RequireContext(ContextType.Guild)]
    public class Tags : XenoBase
    {
        private TagService _service;

        [Command("tag")]
        public async Task Tag(string tagName)
        {
            var tag = _service.FetchTagContentAsync(tagName);
            if (tag != null)
                await ReplyAsync("", embed: new EmbedBuilder().WithColor(new Color(39, 217, 196)).WithDescription(tag).Build());
            else
                await SendErrorEmbedAsync("No tag found with that name.");
        }

        [Command("tags")]
        public async Task ListTags()
        {
            var tags = _service.FetchAllTags().Keys.ToList();
            if (tags.Any())
            {
                string list = string.Join(", ", tags);
                await ReplyAsync($":small_blue_diamond:  **List of all tags:**```\n{list}\n```");
            }
            else
                await ReplyAsync("", embed: Xeno.InfoEmbed("There are no tags."));
            
        }

        [RequirePermission(Permission.GuildAdmin)]
        [Command("tag add"), Alias("tag +")]
        public async Task AddTag(string tagName, [Remainder] string tagContent)
        {
            var tag = await _service.TryAddTagAsync(tagName, tagContent);
            if (tag)
                await ReplyThenRemoveAsync(":ok_hand: Tag added.", TimeSpan.FromSeconds(7));
            else
                await SendErrorEmbedAsync("A tag already exists with that name.");
        }

        [RequirePermission(Permission.GuildAdmin)]
        [Command("tag remove"), Alias("tag delete", "tag -")]
        public async Task RemoveTag(string tagName)
        {
            var tag = await _service.TryRemoveTagAsync(tagName);
            if (tag)
                await ReplyThenRemoveAsync(":ok_hand: Tag removed.", TimeSpan.FromSeconds(7));
            else
                await SendErrorEmbedAsync("No tag exists with that name.");
        }

        [RequirePermission(Permission.GuildAdmin)]
        [Command("tag edit"), Alias("tag delete", "tag =")]
        public async Task EditTag(string tagName, [Remainder] string newTagContent)
        {
            var tag = await _service.TryEditTagAsync(tagName, newTagContent);
            if (tag)
                await ReplyThenRemoveAsync(":ok_hand: Tag edited.", TimeSpan.FromSeconds(7));
            else
                await SendErrorEmbedAsync("No tag exists with that name.");
        }

        public Tags(TagService service)
        {
            _service = service;
        }
    }
}
