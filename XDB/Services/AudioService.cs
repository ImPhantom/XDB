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
using System.IO;
using Newtonsoft.Json;

namespace XDB.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedClients = new ConcurrentDictionary<ulong, IAudioClient>();
        public ConcurrentDictionary<ulong, ulong> ConnectedChannels = new ConcurrentDictionary<ulong, ulong>();

        public List<QueuedVideo> Queue = new List<QueuedVideo>();

        public bool IsPlaying = false;
        private Process FFProcess = null;

        public async Task JoinAudio(IGuild guild, IVoiceChannel channel)
        {
            if (ConnectedClients.TryGetValue(guild.Id, out IAudioClient client))
                return;
            if (channel.Guild.Id != guild.Id)
                return;

            var audioClient = await channel.ConnectAsync();
            if (ConnectedClients.TryAdd(guild.Id, audioClient))
                if(ConnectedChannels.TryAdd(guild.Id, channel.Id))
                    BetterConsole.Log(LogSeverity.Info, "Audio", $"Connected to voice in: {guild.Name} ({channel.Name})");
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (ConnectedClients.TryRemove(guild.Id, out IAudioClient client))
                if(ConnectedChannels.TryRemove(guild.Id, out ulong channelId))
                {
                    await client.StopAsync();
                    BetterConsole.Log(LogSeverity.Info, "Audio", $"Disconnected from voice in: {guild.Name}");
                }
        }

        public async Task StartPlayingAsync(IGuild guild, IUserMessage message, string url)
        {
            var info = GetAudioInfo(url);
            if(info != null)
            {
                Queue.Add(info);
                if (IsPlaying)
                    await message.ModifyAsync(x => x.Content = $":notes:  **Added to queue: `{info.Title}`**");
                else
                    await BeginAudioPlayback(guild, message);
            } else
            {
                await message.Channel.SendMessageAsync("", embed: Xeno.ErrorEmbed("Video duration exceeds the specified limit."));
                await message.DeleteAsync();
                await LeaveAudio(guild);
                return;
            }
        }

        public async Task StartLocalFolderAsync(IGuild guild, IUserMessage message, string foldername, string filename)
        {
            var path = Path.Combine(Xeno.LocalAudioPath, foldername);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                if(filename != null)
                {
                    if (files.Any(x => x.ToLower().Contains(filename.ToLower())))
                    {
                        var file = files.First(x => x.ToLower().Contains(filename.ToLower()));
                        var fn = Path.GetFileName(file);
                        Queue.Add(new QueuedVideo()
                        {
                            Url = file,
                            Title = $"{foldername}/{fn}"
                        });
                        await message.ModifyAsync(x => x.Content = $":notes:  **Added `{foldername}/{fn}` to the queue.**");
                    } else
                    {
                        await message.Channel.SendMessageAsync("", embed: Xeno.ErrorEmbed($"Could not find any file that contained `{filename}` in `{foldername}/`"));
                        await message.DeleteAsync();
                        return;
                    }
                } else
                {
                    foreach (var file in files)
                    {
                        var fn = Path.GetFileName(file);
                        
                        Queue.Add(new QueuedVideo()
                        {
                            Url = file,
                            Title = $"{foldername}/{fn}"
                        });
                    }
                    await message.ModifyAsync(x => x.Content = $":notes:  **Added {files.Count()} files from `{foldername}/` to the queue.**");
                }
                await BeginAudioPlayback(guild, message);
            }
            else
            {
                await message.Channel.SendMessageAsync("", embed: Xeno.ErrorEmbed("No folder with that name."));
                await message.DeleteAsync();
                return;
            }            
        }

        public async Task SkipSongAsync(IGuild guild, IMessageChannel channel)
        {
            await StopPlaying();
            var first = Queue.First();
            Queue.Remove(first);
        }

        

        public Task ClearQueueAsync()
        {
            StopPlaying();
            Queue.Clear();
            return Task.CompletedTask;
        }

        public Task StopPlaying()
        {
            FFProcess.Kill();
            FFProcess = null;
            IsPlaying = false;
            return Task.CompletedTask;
        }

        private QueuedVideo GetAudioInfo(string url)
        {
            var prc = Process.Start(new ProcessStartInfo()
            {
                FileName = @"youtube-dl",
                Arguments = $" -x -g -e --get-duration --get-id {url}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            var title = prc.StandardOutput.ReadLine();
            var videoId = prc.StandardOutput.ReadLine();
            var purl = prc.StandardOutput.ReadLine();
            var duration = prc.StandardOutput.ReadLine();
            if (Xeno.ParseDuration(duration) < Config.Load().AudioDurationLimit)
            {
                return new QueuedVideo()
                {
                    Title = title,
                    Url = purl,
                    Duration = duration,
                    VideoId = videoId
                };
            }
            else
                return null;
        }

        public async Task BeginAudioPlayback(IGuild guild, IUserMessage message)
        {
            if (Queue.Count > 0)
            {
                var song = Queue.First();
                if (ConnectedClients.TryGetValue(guild.Id, out IAudioClient client))
                {
                    FFProcess = CreateStream(song.Url);
                    var volume = Config.Load().AudioVolume;
                    using (var output = FFProcess.StandardOutput.BaseStream)
                    using (var stream = client.CreatePCMStream(AudioApplication.Music, bufferMillis: 2500))
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
                            else
                                await LeaveAudio(guild).ConfigureAwait(false);
                            
                        }
                    }
                }
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1 -aq 4",
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
