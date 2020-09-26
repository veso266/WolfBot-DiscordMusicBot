using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace WolfBot.Commands.Music
{
    class SongEmbedBuilder
    {
        public SongEmbedBuilder(MusicPlayer mp)
        {
            var MusicEmbed = new DiscordEmbedBuilder
            {
                Title = mp.playingSong.Name,
                Color = DiscordColor.Blue
            };

            if (mp.playingSong.mp3tag != null)
            {
                MusicEmbed.Description = $"Artist: {mp.playingSong.mp3tag.Artist}";
                if (!string.IsNullOrWhiteSpace(mp.playingSong.mp3tag.Album))
                    MusicEmbed.Description += $"\nAlbum: { mp.playingSong.mp3tag.Album}";
                if (!string.IsNullOrWhiteSpace(mp.playingSong.mp3tag.Year))
                    MusicEmbed.Description += $"\nYear: { mp.playingSong.mp3tag.Year}";
                if (!string.IsNullOrWhiteSpace(mp.playingSong.mp3tag.Comment))
                    MusicEmbed.Description += $"\nComment: { mp.playingSong.mp3tag.Comment}";
            }

           var musicRespose = mp.ctx.Channel.SendMessageAsync(embed: MusicEmbed);
        }
    }
}
