using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace WolfBot.FileDownloader
{
    static class MathTools
    {
        public static string BytesToString(long byteCount) //https://stackoverflow.com/a/4975942/3368585
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
        /* ARRAY */
        public static T[] RemoveAt<T>(this T[] source, int index) //Metoda ki extenda tabelo ter ji omogoča enostavno brisanje
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
        public static T[] Add<T>(this T[] source, T data2Add) //Metoda ki extenda tabelo ter ji omogoča enostavno dodajanje
        {

            T[] dest = new T[source.Length + 1];
            dest[source.Length + 1] = data2Add; //Tu je kar hočemo dodat na konec tabele
            Array.Copy(source, 0, dest, source.Length, source.Length + 1); //kopiraj na zadnje mesto

            return dest;
        }
        /* ARRAY */
    }

    public class Internet
    {
        WebClient webClient;               // Our WebClient that will be doing the downloading for us
        Stopwatch sw = new Stopwatch();    // The stopwatch which we will be using to calculate the download speed

        public string DownloadSpeed;
        public string DownloadPercent;
        public string DownloadData;
        public int bytesRecieved;

        public void DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and store it.
            DownloadSpeed = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            bytesRecieved = e.ProgressPercentage;

            // Show the percentage.
            DownloadPercent = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            DownloadData = string.Format("{0} MB's / {1} MB's",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

            //write to console our percentage to finish, data to finish and download speed
            Console.WriteLine("{0}  {1}    {2}K/s", DownloadPercent, DownloadData, DownloadSpeed);
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            sw.Reset();

            if (e.Cancelled == true)
            {
                Console.WriteLine("Download has been canceled.");
            }
            else
            {
                Console.WriteLine("Download completed!");
            }
        }


    }
}