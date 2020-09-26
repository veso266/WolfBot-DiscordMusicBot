using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace WolfBot.Attributes
{
    /// <summary>
    /// Defines that usage of this command is restricted to members with specified permissions. This check also verifies that the bot has the same permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RequirePermissionsCustomAttribute : CheckBaseAttribute
    {
        /// <summary>
        /// Gets the permissions required by this attribute.
        /// </summary>
        public Permissions Permissions { get; }

        /// <summary>
        /// Gets or sets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.
        /// </summary>
        public bool IgnoreDms { get; } = true;

        /// <summary>
        /// Message that gets displayed to the user if it doesn't have that permisions (example: You require the {0} permission in order to change settings .
        /// </summary>
        public string deniendMessage { get; set; }

        /// <summary>
        /// Emoji that is before the straing its :x: by default
        /// </summary>
        string emoji = ":x:";

        /// <summary>
        /// Defines that usage of this command is restricted to members with specified permissions. This check also verifies that the bot has the same permissions.
        /// </summary>
        /// <param name="permissions">Permissions required to execute this command.</param>
        /// <param name="deniendMessage">Message that gets displayed to the user if it doesn't have that permisions (example: You require the {0} permission in order to change settings .</param>
        public RequirePermissionsCustomAttribute(Permissions permissions, string deniendMessage = "**You require the {0} permission in order to execute this command**")
        {
            this.Permissions = permissions;
            this.deniendMessage = deniendMessage;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            //Set the emoji
            //emoji = ;

            if (ctx.Guild == null)
                return this.IgnoreDms;

            var usr = ctx.Member;
            if (usr == null)
                return false;
            var pusr = ctx.Channel.PermissionsFor(usr);

            var bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id).ConfigureAwait(false);
            if (bot == null)
                return false;
            var pbot = ctx.Channel.PermissionsFor(bot);
            
            var usrok = ctx.Guild.Owner.Id == usr.Id;
            var botok = ctx.Guild.Owner.Id == bot.Id;

            if (!usrok)
            {
                usrok = (pusr & Permissions.Administrator) != 0 || (pusr & this.Permissions) == this.Permissions;
                if (!usrok)
                    await ctx.Channel.SendMessageAsync(DiscordEmoji.FromName(ctx.Client, emoji) + " " + deniendMessage.Replace("{0}", string.Format("`{0}`", this.Permissions.ToPermissionString()))).ConfigureAwait(false); //Send forbidden message
            }

            if (!botok)
            {
                botok = (pbot & Permissions.Administrator) != 0 || (pbot & this.Permissions) == this.Permissions;
                if (!botok)
                    await ctx.Channel.SendMessageAsync(DiscordEmoji.FromName(ctx.Client, emoji) + " " + deniendMessage.Replace("{0}", string.Format("`{0}`", this.Permissions.ToPermissionString()))).ConfigureAwait(false); //Send forbidden message
            }

            return usrok && botok;
        }
    }
}
