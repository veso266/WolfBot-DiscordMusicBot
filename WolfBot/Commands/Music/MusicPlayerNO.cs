using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext; //for music

using WolfBot.Tools; //some extension methods are in here like GetUsername

namespace WolfBot.Commands.Music
{
    class MusicPlayer
    {
        CommandContext ctx; //Used for sending commands to discord
        public Queue<Song> qs;
        VoiceTransmitStream transmitStream; //Discord Audio buffer


        private TaskCompletionSource<bool> _tcs;
        private CancellationTokenSource _disposeToken;

        private bool Pause
        {
            get => _internalPause;
            set
            {
                new Thread(() => _tcs.TrySetResult(value)).Start();
                _internalPause = value;
            }
        }
        private bool _internalPause;
        /*
        private bool Skip
        {
            get
            {
                bool ret = _internalSkip;
                _internalSkip = false;
                return ret;
            }
            set => _internalSkip = value;
        }
        private bool _internalSkip;
        */

        #region Class Init
        public MusicPlayer(CommandContext ctx)
        {
            this.ctx = ctx;
            qs = new Queue<Song>();

            _tcs = new TaskCompletionSource<bool>();
            _disposeToken = new CancellationTokenSource();

            //Prepare Audio Player
            new Thread(MusicPlay).Start();
        }
        #endregion

        #region Get ffmpeg Audio Procecss
        private static Process GetFfmpeg(string path2file)
        {
            ProcessStartInfo ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-xerror -i \"{path2file}\" -ac 2 -f s16le -ar 48000 pipe:1",
                //UseShellExecute = false,    //TODO: true or false?
                RedirectStandardOutput = true
            };
            return Process.Start(ffmpeg);
        }
        #endregion
        #region Queue functions
        /// <summary>
        /// Lists songs you are having on queue
        /// </summary>
        /// <returns></returns>
        public string listSongs()
        {
            List<string> songs = new List<string>();
            foreach (Song s in qs)
                songs.Add(s.Name);
            if (qs.Count > 0)
                return string.Join('\n', songs);
            else
                return "There are no songs";
        }

        /// <summary>
        /// Skips a song
        /// </summary>
        public void Skip()
        {
            qs.Peek().isSkipped = true;
        }

        public void AddMusic(Song s)
        {
            qs.Enqueue(s); //Add one song to the queue

        }
        public void RemoveMusic(Song s)
        {
            qs.Dequeue();
        }
        public void AddMusic(Queue<Song> qs)
        {
            this.qs = qs; //Load a list of songs
        }



        #endregion
        #region PlaySongs

        public async Task Play()
        {
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                // not enabled
                await ctx.RespondAsync("VNext is not enabled or configured.");
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                // already connected
                await ctx.RespondAsync("Not connected in this guild.");
                return;
            }
            
            await vnc.SendSpeakingAsync(true);
        }

            //Looped Music Play
            private async void MusicPlay()
        {
            bool next = false;

            while (true)
            {
                bool pause = false;
                //Next song if current is over
                if (!next)
                {
                    pause = await _tcs.Task;
                    _tcs = new TaskCompletionSource<bool>();
                }
                else
                {
                    next = false;
                }

                try
                {
                    if (qs.Count == 0)
                    {
                        await ctx.Message.RespondAsync("Nothing :/");
                    }
                    else
                    {
                        if (!pause)
                        {
                            //Get Song
                            var song = qs.Peek();
                            //Update "Playing .."
                            await ctx.Message.RespondAsync(song.Name);
                            
                            await ctx.Message.RespondAsync($"Now playing: **{song.Name}**");

                            //Send audio (Long Async blocking, Read/Write stream)
                            await SendAudio(song.URL);

                            
                            //Finally remove song from playlist
                            qs.Dequeue();
                            
                            next = true;
                        }
                    }
                }
                catch
                {
                    //audio can't be played
                }
            }
        }
        #endregion

        #region AudioPlayer
        //Send Audio with ffmpeg
        private async Task SendAudio(string path)
        {
            //If song is don't play it
            if (qs.Peek().isSkipped)
                await ctx.Message.RespondAsync($"Song: `{qs.Peek().Name}` | Requested by: {UserExtension.GetUsername(ctx.Message.Author)} is skipped");

            //FFmpeg.exe
            Process ffmpeg = GetFfmpeg(path);
            //Read FFmpeg output
            using (Stream output = ffmpeg.StandardOutput.BaseStream)
            {
                    //Adjust?
                    int bufferSize = 1024;
                    int bytesSent = 0;
                    bool fail = false;
                    bool exit = false;
                    byte[] buffer = new byte[bufferSize];

                    while (
                        !qs.Peek().isSkipped &&                     // If Skip is set to true, stop sending and set back to false (with getter)
                        !fail &&                                    // After a failed attempt, stop sending
                        !_disposeToken.IsCancellationRequested &&   // On Cancel/Dispose requested, stop sending
                        !exit                                       // Audio Playback has ended (No more data from FFmpeg.exe)
                            )
                    {
                        try
                        {
                            int read = await output.ReadAsync(buffer, 0, bufferSize, _disposeToken.Token);
                            if (read == 0)
                            {
                                //No more data available
                                exit = true;
                                break;
                            }

                            output.CopyTo(transmitStream); //Copy audio to the Discord buffer

                        if (Pause)
                            {
                                bool pauseAgain;

                                do
                                {
                                    pauseAgain = await _tcs.Task;
                                    _tcs = new TaskCompletionSource<bool>();
                                } while (pauseAgain);
                            }

                            bytesSent += read;
                        }
                        catch (TaskCanceledException)
                        {
                            exit = true;
                        }
                        catch
                        {
                            fail = true;
                            // could not send
                        }
                    //await discord.FlushAsync();
                }
            }
        }
        #endregion
    }
}
