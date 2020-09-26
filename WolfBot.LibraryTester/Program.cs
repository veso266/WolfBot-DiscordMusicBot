using System;
using WolfBot.FileDownloader;
using WolfBot.MP3TAG;

namespace WolfBot.LibraryTester
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] test = WebRequester.Request("http://tv2.partizan.si/YoutubeVideoPlayer/api/audio/song2.mp3", null, null, 128);

            ID3 tag = new ID3(test);

            Console.WriteLine("Tag is available: " + tag.hasTag);
            Console.WriteLine("Artist: " + tag.Artist);
            Console.WriteLine("Title: " + tag.Title);
            
            Console.ReadLine();
        }
    }
}
