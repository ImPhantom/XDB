using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using System.Collections.Generic;
using XDB.Common.Models;
using System.Linq;
using System;
using XDB.Common.Types;

namespace XDB.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        public List<CachedVideo> Queue = new List<CachedVideo>();

        public bool IsPlaying = false;
        private Process FFProcess = null;

        public async Task JoinAudio(IGuild guild, IVoiceChannel channel)
        {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
                return;
            if (channel.Guild.Id != guild.Id)
                return;

            var audioClient = await channel.ConnectAsync();
            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
                BetterConsole.Log(LogSeverity.Info, "Audio", $"Connected to voice in: {guild.Name} ({channel.Name})");
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                BetterConsole.Log(LogSeverity.Info, "Audio", $"Disconnected from voice in: {guild.Name}");
            }
        }
        
        public async Task StartPlayingAsync(IGuild guild, IUserMessage message, CachedVideo video)
        {
            if (IsPlaying)
            {
                Queue.Add(video);
                await message.ModifyAsync(x => x.Content = $":notes:  **Added to queue:** `{video.Title}`");
            }
            else
            {
                Queue.Add(video);
                await BeginAudioPlayback(guild, message);
            }
        }

        public async Task SkipSongAsync(IGuild guild, IMessageChannel channel)
        {
            await StopPlaying();
            var first = Queue.First();
            Queue.Remove(first);
        }

        public Task StopPlaying()
        {
            FFProcess.Kill();
            FFProcess = null;
            IsPlaying = false;
            return Task.CompletedTask;
        }

        private async Task BeginAudioPlayback(IGuild guild, IUserMessage message)
        {
            if (Queue.Count > 0)
            {
                var song = Queue.First();
                if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
                {
                    FFProcess = CreateStream(song.Filename);
                    var volume = Config.Load().AudioVolume;
                    using (var output = FFProcess.StandardOutput.BaseStream)
                    using (var stream = client.CreatePCMStream(AudioApplication.Music))
                    {
                        IsPlaying = true;
                        try
                        {
                            await message.ModifyAsync(x => x.Content = $":loud_sound:  **Now Playing:** `{song.Title}`");
                            byte[] buffer = new byte[3840];
                            int byteCount;
                            while ((byteCount = output.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ScaleVolumeUnsafeNoAlloc(buffer, volume);
                                await stream.WriteAsync(buffer, 0, byteCount);
                            }
                        }
                        finally
                        {
                            await stream.FlushAsync();
                            IsPlaying = false;
                            Queue.Remove(song);
                            if (Queue.Count > 0)
                                await BeginAudioPlayback(guild, message);
                        }
                    }
                }
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public unsafe static byte[] ScaleVolumeUnsafeNoAlloc(byte[] audioSamples, float volume)
        {
            if (audioSamples == null) return null;
            if (audioSamples.Length % 2 != 0) return null;
            if (volume < 0f || volume > 1f) return null;

            if (Math.Abs(volume - 1f) < 0.0001f) return audioSamples;

            int volumeFixed = (int)Math.Round(volume * 65536d);
            int count = audioSamples.Length / 2;
            fixed (byte* srcBytes = audioSamples)
            {
                short* src = (short*)srcBytes;

                for (int i = count; i != 0; i--, src++)
                    *src = (short)(((*src) * volumeFixed) >> 16);
            }
            return audioSamples;
        }
    }
}
