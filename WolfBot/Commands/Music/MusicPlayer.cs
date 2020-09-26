using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext; //for music

using WolfBot.Tools; //some extension methods are in here like GetUsername
using WolfBot.Tools.NativeTools; //To check if ffmpeg is alredy running

namespace WolfBot.Commands.Music
{
    public class MusicPlayer
    {
        public CommandContext ctx; //Used for sending commands to discord
        public Queue<Song> qs;
        VoiceTransmitStream transmitStream; //Discord Audio buffer
        VoiceNextConnection vnc;
        Song NextQueueSong;
        public Song playingSong; //Song that is curently playing
        public Song PrevPlayingSong; //Song that was playing

        string ffmpeg_error = null; //ffmpeg complaints go here

        string filename;

        bool isSongPlaying = false; //Does ffmpeg play a song?

        //FFMpeg proces (our audio player)
        ProcessStartInfo ffmpeg_inf;
        Process ffmpeg;
        void ConfigureFFMpeg()
        {
            ffmpeg_inf = new ProcessStartInfo();
            ffmpeg_inf.FileName = "ffmpeg";
            
            ffmpeg_inf.UseShellExecute = false;
            ffmpeg_inf.RedirectStandardOutput = true;
            ffmpeg_inf.RedirectStandardError = true;
        }
        public MusicPlayer(CommandContext ctx)
        {
            this.ctx = ctx;
            qs = new Queue<Song>();

            //Configure ffmpeg for playing audio
            ConfigureFFMpeg();
        }
        public void AddMusic(Song s)
        {
            qs.Enqueue(s); //Add one song to the queue
            NextQueueSong = s;
        }
        public void RemoveMusic(Song s)
        {
            qs.Dequeue();
        }
        public void AddMusic(Queue<Song> qs)
        {
            this.qs = qs; //Load a list of songs
        }


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
            vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                // already connected
                await ctx.RespondAsync("Not connected in this guild.");
                return;
            }

            // play
            if (vnc.IsPlaying)
                await ctx.RespondAsync($"Added to Queue: `{NextQueueSong.Name}` | Requested by: {UserExtension.GetUsername(ctx.Message.Author)}");
            else
            {

                Exception exc = null;
                await vnc.SendSpeakingAsync(true);
                try
                {
                    filename = qs.Peek().URL; //Set song filename

                    transmitStream = vnc.GetTransmitStream(); //Get Voice transmission stream

                    // check if music input is url and if it is not check if file exists
                    if (!WolfBot.Tools.Network.StringNetworkTools.IsURL(filename))
                    {
                        if (!File.Exists(filename))
                        {
                            // file does not exist
                            await ctx.RespondAsync($"File `{filename}` does not exist.");
                            //Remove the invalid file from the queue
                            qs.Dequeue();
                        }
                    }
                    else
                    {
                        //If the song is not skipped play it
                        if (!qs.Peek().isSkipped)
                        {
                            //await ctx.Message.RespondAsync($"Now Playing `{qs.Peek().Name}` | Requested by: {UserExtension.GetUsername(ctx.Message.Author)}");
                            playingSong = qs.Peek(); //Add playing song

                            //Show song info
                            try
                            {
                                new SongEmbedBuilder(this);
                            }
                            catch
                            {
                                await ctx.RespondAsync("`Resource not found`");
                            }

                            //Play the damm song :)
                            PlayInternal(filename);
                        }
                        else
                        {
                            await ctx.Message.RespondAsync($"Song: `{qs.Peek().Name}` | Requested by: {UserExtension.GetUsername(ctx.Message.Author)} is skipped");
                        }
                    }

                }
                catch (System.InvalidOperationException ext) 
                { 
                    if (ext.Message == "Queue empty")
                    {
                        //Playback is probably over
                    }
                }
                catch (Exception ex) { exc = ex; }
                finally
                {
                    await vnc.SendSpeakingAsync(false);
                    await MusicPlayBackFinished(vnc, "next-song");
                }

                if (exc != null)
                    await ctx.RespondAsync($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`");
            }
        }
        
        public void Pause()
        {

        }
        public void Stop()
        {
            //
            vnc.Disconnect();
        }
        public void Next()
        {

        }
        public void Prev() //Previous song 
        {
            AddMusic(PrevPlayingSong);
        }
        public Task Skip()
        {
            qs.Peek().isSkipped = true;
            return Task.CompletedTask;
        }
        /// <summary>
        /// Lists songs you are having on queue
        /// </summary>
        /// <returns></returns>
        public string listSongs()
        {
            List<string> songs = new List<string>();
            foreach (Song s in qs)
                if (s == playingSong)
                    songs.Add(s.Name + " " + Emojis.Play);
                else
                    songs.Add(s.Name);
            if (qs.Count > 0)
                return string.Join('\n', songs);
            else
                return "There are no songs";
        }
        /// <summary>
        /// Invokes FFMPeg with filename to play
        /// </summary>
        /// <returnes>
        /// Task.Finsiehd when song finishes playing
        /// </returnes>
        void PlayInternal(string filename)
        {
            ffmpeg_inf.Arguments = $"-i \"{filename}\" -ac 2 -f s16le -ar 48000 pipe:1";
            try
            {
                //Only start ffmpeg if song is not playing
                if (!isSongPlaying)
                {
                    ffmpeg = Process.Start(ffmpeg_inf);
                    this.isSongPlaying = true;
                }

                var ffout = ffmpeg.StandardOutput.BaseStream;
                ffout.CopyTo(transmitStream); //Copy audio to the Discord buffer

                ffmpeg_error = ffmpeg.StandardError.ReadToEnd(); //ffmpeg will write complaints here


            }
            catch (Exception ex)
            {
                Console.WriteLine("Something bad happened :(: " + ex.Message);
                return;
            }
        }
        /// <summary>
        /// Called whenever music music playback is stopped
        /// </summary>
        async Task MusicPlayBackFinished(VoiceNextConnection vnc, string command)
        {
            while (vnc.IsPlaying) ; //Do nothing while we are playing, dequeue when we stop

            //Remove already played song from the queue and play the next one
            if (qs.Count > 0)
            {
                PrevPlayingSong = playingSong; //Set previous playing song
                qs.Dequeue(); //Remove last played element from queue
                this.isSongPlaying = false;
                await Play();
            }
            else if (command != "next-song") //If playback ist stopped manualy
            {
                this.isSongPlaying = false;
            }
        }
    }
}
