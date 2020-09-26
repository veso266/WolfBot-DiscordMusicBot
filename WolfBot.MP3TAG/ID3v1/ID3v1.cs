using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* BREZ TEGA NE BO ŠLO :) */
using System.IO;
/* BREZ TEGA NE BO ŠLO :) */

namespace WolfBot.MP3TAG
{
    internal class ID3v1
    {
        //ID3v1 Format 
        /*
        header 	3 	"TAG"
        title 	30 	30 characters of the title
        artist 	30 	30 characters of the artist name
        album 	30 	30 characters of the album name
        year 	4 	A four-digit year
        comment 	28 or 30 	The comment.
        zero-byte   1 	If a track number is stored, this byte contains a binary 0. (ID3v1.1)
        track   1 	The number of the track on the album, or 0. Invalid, if previous byte is not a binary 0. (ID3v1.1)
        genre   1 	Index in a list of genres, or 255 
        */
        //ID3v1 Format

        string fileName;

        internal byte[] _TAGID = new byte[3];      //  3
        internal byte[] _Title = new byte[30];     //  30
        internal byte[] _Artist = new byte[30];    //  30 
        internal byte[] _Album = new byte[30];     //  30 
        internal byte[] _Year = new byte[4];       //  4 
        internal byte[] _Comment = new byte[30];   //  30 
        //public byte[] _zeroByte = new byte[1];   //  1 (If a track number is stored, this byte contains a binary 0)
        internal byte[] _Genre = new byte[1];      //  1

        //public byte[] readBuffer = new byte[128]; //from here we read
        internal byte[] final = new byte[128]; //In here we store a complete ID3v1 TAG

        string Title;
        string Artist;
        string Album;
        string Year;
        string Comment;
        //string Genre;

        int zeroByte;
        int GenreID;
        int Track;

        public bool hasTag = false; //da drugi vejo da se ne matrat če MP3 ni tagiran
        public string artist{ get { return this.Artist.TrimEnd('\0'); } set { this.Artist = value; } }
        public string title { get { return this.Title.TrimEnd('\0'); } set { this.Title = value; } }
        public string album { get { return this.Album.TrimEnd('\0'); } set { this.Artist = value; } }
        public string year { get { return this.Year.TrimEnd('\0'); } set { this.Year = value; } }
        public string comment { get { return this.Comment.TrimEnd('\0'); } set { this.Comment = value; } }
 
        //public int genreID { get { return this.GenreID; } }
        //public int track { get { return this.track; } } 

        private void Initialize_Components()
        {
            Title = "";
            Artist = "";
            Album = "";
            Year = "";
            Comment = "";
            //Genre = "";

            GenreID = 0;
            Track = 0;



        }
        private ID3v1()
        {
            Initialize_Components();
        }

		/// <summary>
		/// Prebere IDv1 značke iz podane datoteke
		/// </summary>
		/// <param name="filename">Ime MP3 datoteke iz katere cemo brat</param>
        public ID3v1(string filename)
        {
            Initialize_Components();
            this.fileName = filename;
        }
        /// <summary>
        /// Prebere IDv1 značko direktno iz byteArraya (v tem arraju mora biti samo 128byte IDV1 značka)
        /// </summary>
        /// <param name="tagdata"></param>
        byte[] znacka = null;
        public ID3v1(byte[] tagdata)
        {
            Initialize_Components();
            this.znacka = tagdata;
        }
        

        //string filePath = @"IDTagReadTest.mp3";
        //string filePath = fileName;
		
		/// <summary>
		/// Prebere IDv1 značke
		/// </summary>
        public void Read()
        {
            _Comment = new byte[28];
            byte[] _zeroByte = new byte[1];
            byte[] _Track = new byte[1];
            _Genre = new byte[1];

            //če značke še nimamo v byte arrayu
            if (znacka == null && znacka.Length < 0)
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    if (fs.Length >= 128)
                    {
                        ID3v1 tag = new ID3v1();
                        fs.Seek(-128, SeekOrigin.End);
                        //fs.Read(readBuffer, 0, 128); //Just so we have some Data in ReadBuffer Array
                        fs.Read(tag._TAGID, 0, tag._TAGID.Length);
                        fs.Read(tag._Title, 0, tag._Title.Length);
                        fs.Read(tag._Artist, 0, tag._Artist.Length);
                        fs.Read(tag._Album, 0, tag._Album.Length);
                        fs.Read(tag._Year, 0, tag._Year.Length);
                        fs.Read(_Comment, 0, _Comment.Length);
                        fs.Read(_zeroByte, 0, _zeroByte.Length);
                        fs.Read(_Track, 0, _Track.Length);
                        fs.Read(_Genre, 0, tag._Genre.Length);
                        string theTAGID = Encoding.Default.GetString(tag._TAGID);

                        if (theTAGID.Equals("TAG"))
                        {
                            hasTag = true;
                            Title = Encoding.Default.GetString(tag._Title);
                            Artist = Encoding.Default.GetString(tag._Artist);
                            Album = Encoding.Default.GetString(tag._Album);
                            Year = Encoding.Default.GetString(tag._Year);
                            Comment = Encoding.Default.GetString(_Comment);
                            //Genre = Encoding.Default.GetString(tag._Genre);
                            zeroByte = _zeroByte[0];
                            GenreID = _Genre[0];
                            Track = _Track[0];
                        }
                    }
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(znacka))
                {
                    ID3v1 tag = new ID3v1();
                    ms.Read(tag._TAGID, 0, tag._TAGID.Length);
                    ms.Read(tag._Title, 0, tag._Title.Length);
                    ms.Read(tag._Artist, 0, tag._Artist.Length);
                    ms.Read(tag._Album, 0, tag._Album.Length);
                    ms.Read(tag._Year, 0, tag._Year.Length);
                    ms.Read(_Comment, 0, _Comment.Length);
                    ms.Read(_zeroByte, 0, _zeroByte.Length);
                    ms.Read(_Track, 0, _Track.Length);
                    ms.Read(_Genre, 0, tag._Genre.Length);
                    string theTAGID = Encoding.Default.GetString(tag._TAGID);

                    if (theTAGID.Equals("TAG"))
                    {
                        hasTag = true;
                        Title = Encoding.Default.GetString(tag._Title);
                        Artist = Encoding.Default.GetString(tag._Artist);
                        Album = Encoding.Default.GetString(tag._Album);
                        Year = Encoding.Default.GetString(tag._Year);
                        Comment = Encoding.Default.GetString(_Comment);
                        //Genre = Encoding.Default.GetString(tag._Genre);
                        zeroByte = _zeroByte[0];
                        GenreID = _Genre[0];
                        Track = _Track[0];
                    }
                }
            }
        }



        public void Write()
        {
            //File.SetAttributes(filePath, FileAttributes.Normal);
            using (FileStream fsStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryWriter fs = new BinaryWriter(fsStream))
                {
                    if (fsStream.Length >= 128)
                    {
                        GenreID = 2;
                        Track = 0;

                        ID3v1 tag = new ID3v1();
                        fs.Seek(-128, SeekOrigin.End); //Goto 128 Byte where we start to write our tags
                        tag._TAGID = Encoding.Default.GetBytes("TAG"); //ID3v1: Always write "TAG" at the beggining

                        tag._Artist = Encoding.Default.GetBytes(Artist); //Write to Artist ByteArray
                        tag._Title = Encoding.Default.GetBytes(Title); //Write to Title ByteArray
                        tag._Album = Encoding.Default.GetBytes(Album); //Write to Album ByteArray
                        //tag._Genre = Encoding.Default.GetBytes(Genre); //Write to Genre ByteArray
                        tag._Year = Encoding.Default.GetBytes(Year); //Write to Year ByteArray
                        tag._Comment = Encoding.Default.GetBytes(Comment); //Write to Comment ByteArray

                        //Init to 0 then copy on top
                        for (int i = 0; i < tag.final.Length; i++)
                        {
                            tag.final[i] = 0x00;
                        }

                        //Copy bytes to final ByteArray

                        Array.Copy(tag._TAGID, tag.final, tag._TAGID.Length); //Copy TAG (3 bytes) to Array
                        Array.Copy(tag._Title, 0, tag.final, 3, tag._Title.Length); //Copy Title (30 bytes) to Array
                        Array.Copy(tag._Artist, 0, tag.final, 30 + 3, tag._Artist.Length); //Copy Artist (30 bytes) to Array
                        Array.Copy(tag._Album, 0, tag.final, 30 + 3 + 30, tag._Album.Length); //Copy Album (30 bytes) to Array
                        Array.Copy(tag._Year, 0, tag.final, 30 + 3 + 30 + 30, tag._Year.Length); //Copy Year (4 bytes) to Array
                        Array.Copy(tag._Comment, 0, tag.final, 30 + 3 + 30 + 30 + 4, tag._Comment.Length); //Copy Comment (30 bytes) to Array
                        //Array.Copy(tag.Genre, 0, tag.final, 30 + 3 + 30 + 30 + 4 + 30, tag.Genre.Length); //Copy Genre (1 byte) to Array
                        tag.final[126] = Convert.ToByte(Track);
                        tag.final[127] = Convert.ToByte(GenreID);


                        //Console.WriteLine(Encoding.Default.GetString(tag.final)); //So I can see what I am doing

                        //write the final array
                        fs.Write(tag.final, 0, tag.final.Length);

                    }
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("DONE");
                }
            }
        }
        public byte[] WriteBytes(string _Artist, string _Title, string _Album, string _Year, string _Comment, int _GenreID, int _Track)
        {
            ID3v1 tag = new ID3v1(); //Instalize the class

            //We set some variables which we will write
            Artist = _Artist;
            Title = _Title;
            Album = _Album;
            Year = _Year;
            Comment = _Comment;
            GenreID = _GenreID;
            Track = _Track;
            //We set some variables which we will write

            tag._TAGID = Encoding.Default.GetBytes("TAG"); //ID3v1: Always write "TAG" at the beggining

            tag._Artist = Encoding.Default.GetBytes(Artist); //Write to Artist ByteArray
            tag._Title = Encoding.Default.GetBytes(Title); //Write to Title ByteArray
            tag._Album = Encoding.Default.GetBytes(Album); //Write to Album ByteArray
            //tag._Genre = Encoding.Default.GetBytes(Genre); //Write to Genre ByteArray
            tag._Year = Encoding.Default.GetBytes(Year); //Write to Year ByteArray
            tag._Comment = Encoding.Default.GetBytes(Comment); //Write to Comment ByteArray
            //Init to 0 then copy on top
            for (int i = 0; i < tag.final.Length; i++)
            {
                tag.final[i] = 0x00;
            }
            //Copy bytes to final ByteArray
            Array.Copy(tag._TAGID, tag.final, tag._TAGID.Length); //Copy TAG (3 bytes) to Array
            Array.Copy(tag._Title, 0, tag.final, 3, tag._Title.Length); //Copy Title (30 bytes) to Array
            Array.Copy(tag._Artist, 0, tag.final, 30 + 3, tag._Artist.Length); //Copy Artist (30 bytes) to Array
            Array.Copy(tag._Album, 0, tag.final, 30 + 3 + 30, tag._Album.Length); //Copy Album (30 bytes) to Array
            Array.Copy(tag._Year, 0, tag.final, 30 + 3 + 30 + 30, tag._Year.Length); //Copy Year (4 bytes) to Array
            Array.Copy(tag._Comment, 0, tag.final, 30 + 3 + 30 + 30 + 4, tag._Comment.Length); //Copy Year (30 bytes) to Array
            //Array.Copy(tag.Genre, 0, tag.final, 30 + 3 + 30 + 30 + 4 + 30, tag.Genre.Length); //Copy Year (1 byte) to Array
            tag.final[126] = Convert.ToByte(Track);
            tag.final[127] = Convert.ToByte(GenreID);

            //return final ByteArray
            return tag.final;
        }
    }
}
