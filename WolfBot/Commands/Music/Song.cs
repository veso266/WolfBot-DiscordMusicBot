using System;
using System.Collections.Generic;
using System.Text;
using WolfBot.MP3TAG;
using WolfBot.FileDownloader;

using WolfBot.Tools.Network;

namespace WolfBot.Commands.Music
{
    public class Song
    {
        //public int ID { get; private set; }
        public string Name { get; private set; }
        public string URL { get; private set; }
        public bool isSkipped { get; set; } = false; //also used when song finished playing
        ID3 _ID3 { get; }
        public ID3 mp3tag { get; private set; } = null;

        public Song(string file)
        {
            if (StringNetworkTools.IsURL(file))
            {
                //Setup MP3 IDv1 TAG Reader
                //First we get our MP3TAG from remote file

                byte[] id3data = WebRequester.Request(file, null, null, 128);


                this._ID3 = new ID3(id3data);
                if (_ID3.hasTag)
                {
                    this.mp3tag = _ID3;
                    this.Name = $"{mp3tag.Artist} - {mp3tag.Title}";
                }
                else
                {
                    string href = StringNetworkTools.FileFromURL(file);
                    this.Name = href.Split('.')[0];
                }
                this.URL = file;
            }
            else
            {
                this.Name = file.Split('.')[0];
                this.URL = file;
                this.mp3tag = null;
            }
        }
    }
}
