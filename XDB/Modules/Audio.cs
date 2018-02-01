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
            if (_service.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    var message = await ReplyAsync(":hourglass:  Fetching video information...");
                    await _service.StartPlayingAsync(Context.Guild, message, ParseVideo(song));
                }
                else
                    await ReplyAsync("", embed: Xeno.ErrorEmbed("This command requires you to be in the bots voice channel."));
            }
        }

        [RequirePermission(Permission.GuildAdmin)]
        [Command("local", RunMode = RunMode.Async)]
        public async Task Local(string foldername, string filename = "")
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            if (_service.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    var message = await ReplyAsync(":hourglass:  Fetching files from local directory...");
                    await _service.StartLocalFolderAsync(Context.Guild, message, foldername, filename);
                }
                else
                    await ReplyAsync("", embed: Xeno.ErrorEmbed("This command requires you to be in the bots voice channel."));
            }
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

        [RequirePermission(Permission.XDBModerator)]
        [Command("skip", RunMode = RunMode.Async)]
        public async Task Skip()
        {
            if (_service.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _service.SkipSongAsync(Context.Guild, Context.Channel);
                }
                else
                    await ReplyAsync("", embed: Xeno.ErrorEmbed("This command requires you to be in the bots voice channel."));
            }
        }

        [RequirePermission(Permission.XDBAdministrator)]
        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            if (_service.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _service.StopPlaying();
                    await ReplyAsync(":ok_hand:");
                }
                else
                    await ReplyAsync("", embed: Xeno.ErrorEmbed("This command requires you to be in the bots voice channel."));
            }
        }

        [RequirePermission(Permission.XDBAdministrator)]
        [Command("clearqueue", RunMode = RunMode.Async)]
        public async Task ClearQueue()
        {
            await _service.ClearQueueAsync();
            await ReplyAsync(":ok_hand:");
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
            if (_service.ConnectedChannels.TryGetValue(Context.Guild.Id, out ulong channelId))
            {
                if ((Context.User as IVoiceState).VoiceChannel.Id == channelId)
                {
                    await _service.LeaveAudio(Context.Guild);
                    await ReplyAsync(":ok_hand:");
                }
                else
                    await ReplyAsync("", embed: Xeno.ErrorEmbed("This command requires you to be in the bots voice channel."));
            }
            
        }

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
