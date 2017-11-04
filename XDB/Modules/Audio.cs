using Discord.Commands;
using XDB.Services;
using System.Threading.Tasks;
using Discord;
using XDB.Common;
using XDB.Common.Attributes;
using XDB.Common.Types;
using System.Text;

namespace XDB.Modules
{
    [RequireContext(ContextType.Guild)]
    public class Audio : ModuleBase
    {
        private readonly AudioService _service;
        private readonly CachingService _caching;

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string song)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            var message = await ReplyAsync(":hourglass:  Downloading/Validating...");
            var video = await _caching.CacheVideo(ParseVideo(song));
            if (video == null)
            {
                await message.DeleteAsync();
                await ReplyAsync("", embed: Xeno.ErrorEmbed("Video duration exceeds the specified limit."));
                return;
            }
            await _service.StartPlayingAsync(Context.Guild, message, video);       
        }

        [Command("queue", RunMode = RunMode.Async)]
        public async Task Queue()
        {
            int inc = 1;
            var list = new StringBuilder();
            foreach (var song in _service.Queue)
            {
                list.AppendLine($"**{inc}.)** [{song.Title}]({song.Url})");
                inc++;
            }
            await ReplyAsync("", embed: new EmbedBuilder().WithTitle("Songs in Queue:").WithDescription(list.ToString()).WithColor(new Color(33, 171, 217)).Build());
        }

        [RequireModerator]
        [Command("skip", RunMode = RunMode.Async)]
        public async Task Skip()
            => await _service.SkipSongAsync(Context.Guild, Context.Channel);

        [RequireAdministrator]
        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
            => await _service.StopPlaying();

        [RequireAdministrator]
        [Command("volume", RunMode = RunMode.Async)]
        public async Task Volume([Remainder] float newVolume)
        {
            if (newVolume > 1f || newVolume < 0f)
            {
                await ReplyAsync("", embed: Xeno.ErrorEmbed("Volume has to be within 0 => 1"));
                return;
            }
            var config = Config.Load();
            var before = config.AudioVolume;
            config.AudioVolume = newVolume;
            config.Save();
            await ReplyAsync($":white_small_square:  Volume: `{before}f` => `{config.AudioVolume}f`");
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave()
            => await _service.LeaveAudio(Context.Guild);

        [RequireGuildAdmin]
        [Command("v_debug", RunMode = RunMode.Async)]
        public async Task Status()
        {
            var embed = new EmbedBuilder().WithColor(new Color(0, 188, 140)).AddField("Total Cache Size:", _caching.GetCacheFolderSize().ToFormalSize()).AddField("Total Files Cached:", _caching.GetCachedFileCount()).WithCurrentTimestamp();
            await ReplyAsync("", embed: embed.Build());
        }

        [RequireOwner]
        [Command("cac", RunMode = RunMode.Async)]
        public async Task ClearAllCache()
        {
            var folderSize = _caching.GetCacheFolderSize().ToFormalSize();
            var fileCount = _caching.GetCachedFileCount();
            await _caching.ClearCache();
            await ReplyAsync("", embed: Xeno.InfoEmbed($"Success! Deleted {fileCount} files, emptying {folderSize} of space."));
        }

        private string ParseVideo(string input)
        {
            if (input.StartsWith("http"))
                return input;
            else
                return $"\"ytsearch:{input}\"";
        }

        public Audio(AudioService service, CachingService caching)
        {
            _service = service;
            _caching = caching;
        }
    }
}
