using System;
using System.Collections.Generic;
using System.Text;

namespace WolfBot.Tools.Network
{
    public class StringNetworkTools
    {
        /// <summary>
        /// Checks if a string is a valid url
        /// </summary>
        /// <param name="uriName"></param>
        /// <returns></returns>
        public static bool IsURL(string uriName)
        {
            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static string FileFromURL(string url)
        {
            int lastSlash = url.LastIndexOf('/');
            return url.Substring(lastSlash + 1);
        }
    }
}
