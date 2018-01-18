using Discord.Commands;
using XDB.Services;
using System.Threading.Tasks;
using Discord;
using XDB.Common.Attributes;
using XDB.Common.Types;
using System.Text;
using System.Linq;

namespace XDB.Modules
{
    [Summary("Audio")]
    [RequireContext(ContextType.Guild)]
    public class Audio : ModuleBase
    {
        private readonly AudioService _service;

        [Ratelimit(3, 1, Measure.Minutes)]
        [Command("play", RunMode = RunMode.Async), Summary("Plays a song from a youtube link/search query.")]
        public async Task Play([Remainder] string song)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            var message = await ReplyAsync(":hourglass:  Fetching video information...");
            await _service.StartPlayingAsync(Context.Guild, message, ParseVideo(song));
        }

        [Command("song", RunMode = RunMode.Async), Summary("Gets the currently playing song.")]
        public async Task Song()
        {
            if (_service.IsPlaying)
            {
                var song = _service.Queue.First();
                await ReplyAsync("", embed: new EmbedBuilder().WithTitle("Currently Playing:").WithDescription($"[{song.Title}](http://www.youtube.com/watch?v={song.VideoId})").WithColor(Xeno.RandomColor()).Build());
            }
            else
                await ReplyAsync("", embed: Xeno.ErrorEmbed("The bot is not playing any music."));
        }

        [Command("queue", RunMode = RunMode.Async)]
        public async Task Queue()
        {
            int inc = 1;
            var list = new StringBuilder();
            foreach (var song in _service.Queue)
            {
                list.AppendLine($"**{inc}.)** [{song.Title}](http://www.youtube.com/watch?v={song.VideoId})");
                inc++;
            }
            await ReplyAsync("", embed: new EmbedBuilder().WithTitle("Songs in Queue:").WithDescription(list.ToString()).WithColor(Xeno.RandomColor()).Build());
        }

        [RequireModerator]
        [Command("skip", RunMode = RunMode.Async)]
        public async Task Skip()
            => await _service.SkipSongAsync(Context.Guild, Context.Channel);

        [RequireAdministrator]
        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            await _service.StopPlaying();
            await ReplyAsync(":ok_hand:");
        }

        [RequireAdministrator]
        [Command("clearqueue", RunMode = RunMode.Async)]
        public async Task ClearQueue()
        {
            await _service.ClearQueueAsync();
            await ReplyAsync(":ok_hand:");
        }

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
            await ReplyAsync($":white_small_square:  **Volume:** `{before}f` => `{config.AudioVolume}f`");
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave()
            => await _service.LeaveAudio(Context.Guild);

        private string ParseVideo(string input)
        {
            if (input.StartsWith("http"))
                return input;
            else
                return $"\"ytsearch:{input}\"";
        }

        public Audio(AudioService service)
        {
            _service = service;
        }
    }
}
