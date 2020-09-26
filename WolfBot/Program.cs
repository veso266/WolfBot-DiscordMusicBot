using System;
using System.IO;
using System.Reflection;

namespace WolfBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            try
            {
                bot.RunAsync().GetAwaiter().GetResult(); //Start the bot
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File: " + ex.FileName + " Not found");
            }
            Console.ReadLine();
        }
    }
}
