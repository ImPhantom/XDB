using Discord.Commands;
using XDB.Services;
using System.Threading.Tasks;
using Discord;
using XDB.Common.Attributes;
using XDB.Common.Types;
using System.Text;
using System.Linq;
using XDB.Common;

namespace XDB.Modules
{
    [Summary("Audio")]
    [RequireContext(ContextType.Guild)]
    public class Audio : XenoBase
    {
        private readonly AudioService _audio;

        [Ratelimit(3, 1, Measure.Minutes)]
        [Command("play", RunMode = RunMode.Async), Summary("Plays a song from a youtube link/search query.")]
        public async Task Play([Remainder] string song)
        {
            await _audio.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            if (_audio.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    var message = await ReplyAsync(":hourglass:  Fetching video information...");
                    await _audio.StartPlayingAsync(Context.Guild, message, ParseVideo(song));
                }
                else
                    await SendErrorEmbedAsync("This command requires you to be in the bots voice channel.");
            }
        }

        [RequirePermission(Permission.GuildAdmin)]
        [Command("local", RunMode = RunMode.Async)]
        public async Task Local(string foldername, string filename = null)
        {
            await _audio.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            if (_audio.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    var message = await ReplyAsync(":hourglass:  Fetching files from local directory...");
                    await _audio.StartLocalFolderAsync(Context.Guild, message, foldername, filename);
                }
                else
                    await SendErrorEmbedAsync("This command requires you to be in the bots voice channel.");
            }
        }

        [Command("song", RunMode = RunMode.Async), Summary("Gets the currently playing song.")]
        public async Task Song()
        {
            if (_audio.IsPlaying)
            {
                var song = _audio.Queue.First();
                await ReplyAsync("", embed: new EmbedBuilder().WithTitle("Currently Playing:").WithDescription($"[{song.Title}](http://www.youtube.com/watch?v={song.VideoId})").WithColor(Xeno.RandomColor()).Build());
            }
            else
                await SendErrorEmbedAsync("The bot is not playing any music.");
        }

        [Command("queue", RunMode = RunMode.Async)]
        public async Task Queue()
        {
            int inc = 1;
            var list = new StringBuilder();
            foreach (var song in _audio.Queue)
            {
                list.AppendLine($"**{inc}.)** [{song.Title}](http://www.youtube.com/watch?v={song.VideoId})");
                inc++;
            }
            await ReplyAsync("", embed: new EmbedBuilder().WithTitle("Songs in Queue:").WithDescription(list.ToString()).WithColor(Xeno.RandomColor()).Build());
        }

        [RequirePermission(Permission.XDBModerator)]
        [Command("skip", RunMode = RunMode.Async)]
        public async Task Skip()
        {
            if (_audio.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _audio.SkipSongAsync(Context.Guild, Context.Channel);
                    await ReplyThenRemoveAsync(":ok_hand:");
                }
                else
                    await SendErrorEmbedAsync("This command requires you to be in the bots voice channel.");
            }
        }

        [RequirePermission(Permission.XDBAdministrator)]
        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            if (_audio.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _audio.StopPlaying();
                    await ReplyThenRemoveAsync(":ok_hand:");
                }
                else
                    await SendErrorEmbedAsync("This command requires you to be in the bots voice channel.");
            }
        }

        [RequirePermission(Permission.XDBAdministrator)]
        [Command("clearqueue", RunMode = RunMode.Async)]
        public async Task ClearQueue()
        {
            await _audio.ClearQueueAsync();
            await ReplyThenRemoveAsync(":ok_hand:");
        }

        [RequirePermission(Permission.XDBAdministrator)]
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
        {
            if (_audio.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _audio.LeaveAudio(Context.Guild);
                    await ReplyThenRemoveAsync(":ok_hand:");
                }
                else
                    await SendErrorEmbedAsync("This command requires you to be in the bots voice channel.");
            }
            
        }

        private string ParseVideo(string input)
        {
            if (input.StartsWith("http"))
                return input;
            else
                return $"\"ytsearch:{input}\"";
        }

        public Audio(AudioService audio)
        {
            _audio = audio;
        }
    }

    
}
