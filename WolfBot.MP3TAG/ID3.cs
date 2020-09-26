using System;
using System.Collections.Generic;
using System.Text;

namespace WolfBot.MP3TAG
{
    public class ID3
    {
        ID3v1 tag;


        public bool hasTag { get { return tag.hasTag; } }
        public string Artist { get { return tag.artist; }}
        public string Title { get { return tag.title; }}
        public string Album { get { return tag.album; }}
        public string Year { get { return tag.year; }}
        public string Comment { get { return tag.comment; }}

        public ID3(byte[] tagData)
        {
            tag = new ID3v1(tagData);
            tag.Read();
        }
    }
}
