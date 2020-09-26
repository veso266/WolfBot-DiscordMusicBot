using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using WolfBot.Tools.Network;
using WolfBot.Attributes;
using WolfBot.Handlers.Dialogue;
using WolfBot.Handlers.Dialogue.Steps;

namespace WolfBot.Commands
{
    class NetworkCommands : BaseCommandModule
    {
        /// <summary>
        /// Returns the latency between the server websocekt and between reading messages
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("ping")]
        [Description("Returns WolfPong")]
        [RequireCategories(ChannelCheckMode.Any, "Text Channels")]
        public async Task Ping(CommandContext ctx)
        {
            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            string welcome = "WolfPong: ";
            string fmt;

            //Calculate latency
            TimeSpan msg_latency = DateTime.UtcNow - ctx.Message.CreationTimestamp.UtcDateTime; //Calculate messageRecieve latency
            fmt = string.Format("Message latency {0} seconds", msg_latency.Seconds + msg_latency.Milliseconds / 1000000);
            fmt += string.Format("\nWebsocket latency {0} ms", ctx.Client.Ping);

            /* Trigger the Typing... in discord */
            await ctx.TriggerTypingAsync();

            //Send the message "Ping" with the latency of the bot
            await ctx.RespondAsync($" {welcome} {emoji} \n{fmt}");
            //await ctx.Channel.SendMessageAsync(welcome + " " + fmt).ConfigureAwait(false);
        }
        /// <summary>
        /// Ads 2 numbers together
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="prvaStevilka"></param>
        /// <param name="drugaStevilka"></param>
        /// <returns></returns>
        [Command("plus")]
        [Description("Adss 2 numbers together")]
        [RequirePermissionsCustom(DSharpPlus.Permissions.Administrator)]
        //[RequireCategories(ChannelCheckMode.Any, "User")]
        public async Task Plus(CommandContext ctx, [Description("First number")] int prvaStevilka, [Description("Second number")] int drugaStevilka)
        {
            /* Trigger the Typing... in discord */
            await ctx.TriggerTypingAsync();

            //Send the message "Ping" with the latency of the bot
            await ctx.Channel.SendMessageAsync((prvaStevilka + drugaStevilka).ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Calculator
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="what2calculate"></param>
        /// <returns></returns>
        [Command("calculate")]
        [Description("Calculates 2 numbers together")]
        //[RequireCategories(ChannelCheckMode.Any, "User")]
        public async Task Plus(CommandContext ctx, [Description("Expresion to calculate")] string what2calculate)
        {
            /* Trigger the Typing... in discord */
            await ctx.TriggerTypingAsync();


            //Figure our if its division, addition, subtraction, etc
            int result = 0;
            if(what2calculate.Contains('+'))
            {
                string[] what2caltulateArr = what2calculate.Split('+');
                int prvaStevilka = Convert.ToInt32(what2caltulateArr[0]); //Left side of expresion
                int drugaStevilka = Convert.ToInt32(what2caltulateArr[1]); //Right side of expresion

                result = prvaStevilka + drugaStevilka;
            }
            else if (what2calculate.Contains('-'))
            {
                string[] what2caltulateArr = what2calculate.Split('-');
                int prvaStevilka = Convert.ToInt32(what2caltulateArr[0]); //Left side of expresion
                int drugaStevilka = Convert.ToInt32(what2caltulateArr[1]); //Right side of expresion

                result = prvaStevilka - drugaStevilka;
            }
            else if (what2calculate.Contains('*'))
            {
                string[] what2caltulateArr = what2calculate.Split('*');
                int prvaStevilka = Convert.ToInt32(what2caltulateArr[0]); //Left side of expresion
                int drugaStevilka = Convert.ToInt32(what2caltulateArr[1]); //Right side of expresion

                result = prvaStevilka * drugaStevilka;
            }
            else if (what2calculate.Contains('/'))
            {
                string[] what2caltulateArr = what2calculate.Split('/');
                int prvaStevilka = Convert.ToInt32(what2caltulateArr[0]); //Left side of expresion
                int drugaStevilka = Convert.ToInt32(what2caltulateArr[1]); //Right side of expresion

                result = prvaStevilka / drugaStevilka;
            }

            await ctx.RespondAsync(result.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns Radio Station name based on its PI Code
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="PICode"></param>
        /// <returns></returns>
        [Command("PI")]
        [Description("Returns Radio Station name based on its PI Code")]
        //[RequireRoles(RoleCheckMode.Any, "Owner")]
        public async Task GetPI(CommandContext ctx, [Description("PI Code (example: 9201) ")] string PICode)
        {
            /* Trigger the Typing... in discord */
            await ctx.TriggerTypingAsync();

            string stations = await NetworkTools.FMListPICode(PICode);

            //Send the message
            await ctx.RespondAsync(stations).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns your External IP Address
        /// </summary>
        /// <returns></returns>
        [Command("ip")]
        [Description("Returns your External IP Address")]
        [RequirePermissionsCustom(DSharpPlus.Permissions.Administrator)]
        public async Task GetExternalIP(CommandContext ctx)
        {
            /* Trigger the Typing... in discord */
            await ctx.TriggerTypingAsync();

            string ip = await NetworkTools.GetExternalIP();

            //Send the message
            await ctx.RespondAsync(string.Format("Bots IP Address is: `{0}`", ip)).ConfigureAwait(false);
        }
    }
}
