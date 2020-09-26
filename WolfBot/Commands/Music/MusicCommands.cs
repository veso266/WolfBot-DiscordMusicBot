using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

using WolfBot.Attributes;
using WolfBot.Commands.Music;

namespace WolfBot.Commands
{
    class MusicCommands : BaseCommandModule
    {
        int SongID = 1;
        MusicPlayer player;
        [Command("join")]
        [RequirePermissionsCustom(Permissions.UseVoice)]
        public async Task Join(CommandContext ctx)
        {
            //Initialize music player
            player = new MusicPlayer(ctx);

            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                await ctx.RespondAsync("Already connected in this guild.");
                throw new InvalidOperationException("Already connected in this guild.");
            }

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                await ctx.RespondAsync("You need to be in a voice channel.");
                throw new InvalidOperationException("You need to be in a voice channel.");
            }

            vnc = await vnext.ConnectAsync(chn);
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:")); //👌
        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                await ctx.RespondAsync("Not connected in this guild.");
                return;
            }

            vnc.Disconnect();
            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok_hand:")); //👌
        }

        [Command("play"), Description("Plays an audio file.")]
        public async Task Play(CommandContext ctx, [RemainingText, Description("Full path to the file to play.")] string filename)
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
                await Join(ctx); //If we aren't already joined in the guid then join it
                //await ctx.RespondAsync("Not connected in this guild.");
                //return;
            }

            //Add songs
            if (!string.IsNullOrWhiteSpace(filename)) //If we only type !play
            {
                Song song = new Song(filename);
                player.AddMusic(song);
            }

            //Start Playing
            if (player.qs.Count <= 0)
            {
                await ctx.RespondAsync("Add a song first");
            }
            await player.Play();

        }
        [Command("stop"), Description("Stops currently playing file.")]
        public async Task Stop(CommandContext ctx)
        {
            await ctx.Message.RespondAsync("Playback is stopped");
            player.Stop();
        }

        [Command("list-songs"), Description("List all the songs in queue")]
        public async Task ListSongs(CommandContext ctx)
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

            await ctx.RespondAsync(player.listSongs());
        }

        [Command("skip"), Description("Skips a song that is curently in queue")]
        public async Task Skip(CommandContext ctx)
        {
            await player.Skip();
        }
    }
}
