using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WolfBot.Tools.Network
{
    public class NetworkTools
    {
        /// <summary>
        /// Connect to specified website
        /// </summary>
        /// <param name="url">URL TO WEBSTIE</param>
        /// <returns></returns>
        public static async Task<string> GetWebsite(string url)
        {
            using (HttpClient client = new HttpClient())
                return await client.GetStringAsync(url);
        }
        /// <summary>
        /// Returnes fm stations Code based on its PI CODE
        /// </summary>
        /// <param name="PI"></param>
        /// <returns></returns>
        public static async Task<string> FMListPICode(string PI)
        {
            string response = await GetWebsite(string.Format("https://www.fmlist.org/pi.php?pi={0}", PI));
            return response;
        }

        public static async Task<string> GetExternalIP()
        {
            string response = await GetWebsite("https://myexternalip.com/raw");
            return response;
        }
    }
}
