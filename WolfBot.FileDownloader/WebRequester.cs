using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace WolfBot.FileDownloader
{
    public class WebRequester
    {
        /// <summary>
        /// Requests online resource in chunks
        /// </summary>
        /// <param name="url">URL to request</param>
        /// <param name="start">where to start reading</param>
        /// <param name="end">where to stop reading</param>
        /// <param name="bytestilEnd">How many bytes to read til the end (Example: If you are reading IDv1 MP3 TAG you read 128 bytes before file end)</param>
        /// <returns>bytearray containing the response</returns>
        public static byte[] Request(string url, long? start=null, long? end=null, long? bytestilEnd=null)
        {
            long filesize = 0;
            long Lstart = 0;
            if (end == null)
                filesize = ResponseSize(url);
            else
                filesize = end.Value;

            if (start == null && bytestilEnd != null)
            {
                Lstart = filesize - bytestilEnd.Value;
            }
            else
                Lstart = start.Value;


            byte[] responseinBytes = new byte[filesize-Lstart];

            HttpWebRequest req = (HttpWebRequest)System.Net.WebRequest.Create(url);
            req.Method = "GET";
            if (filesize > 0)
            {
                req.AddRange(Convert.ToInt32(Lstart), Convert.ToInt32(filesize));
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


                using (Stream responseStream = resp.GetResponseStream())
                {
                    responseStream.Read(responseinBytes, 0, (int)responseinBytes.Length);
                }
            }
            else
                responseinBytes[0] = 0; //We don't have anything to give you sorry 
            return responseinBytes;
        }

        /// <summary>
        /// Gets Filesize from the specified server endpoint
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Filezise in bytes</returns>
        public static long ResponseSize(string url)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            long responseLength = 0;
            try
            {
                System.Net.WebResponse resp = req.GetResponse();
                responseLength = resp.ContentLength;
                resp.Close();
            }catch
            {
                
            }
            return responseLength;
        }
    }
}
