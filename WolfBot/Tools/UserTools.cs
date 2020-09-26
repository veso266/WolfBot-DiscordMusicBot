using System;
using DSharpPlus.Entities;

namespace WolfBot.Tools
{
    // Extension methods must be defined in a static class.
    public static class UserExtension
    {
        /// <summary>
        /// This method will return username from discord user string (example: from this: Member 377421711232204812; veso266#8329 (veso266))
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Discord username (example: veso266#8329)</returns>
        public static string GetUsername(DiscordUser str)
        {
            string longUsername = str.ToString(); //377421711232204812; veso266#8329 (veso266)
            string shorterUsername = longUsername.Split(';')[1]; // veso266#8329 (veso266)
            string finalUsername = shorterUsername.Substring(1, shorterUsername.IndexOf('(')-2); //veso266#8329
            return finalUsername;
        }
        
    }
}
