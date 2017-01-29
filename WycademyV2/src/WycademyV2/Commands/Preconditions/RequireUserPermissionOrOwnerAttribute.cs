using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Preconditions
{
    public class RequireUserPermissionOrOwnerAttribute : PreconditionAttribute /* That's a long name */
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        public RequireUserPermissionOrOwnerAttribute(GuildPermission guildperm)
        {
            GuildPermission = guildperm;
            ChannelPermission = null;
        }

        public RequireUserPermissionOrOwnerAttribute(ChannelPermission channelperm)
        {
            ChannelPermission = channelperm;
            GuildPermission = null;
        }

        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            // Always succeed if the calling user is the bot owner.
            if (context.User.Id == (await context.Client.GetApplicationInfoAsync()).Owner.Id) return PreconditionResult.FromSuccess();

            // If guildUser is null, then the command is being executed from a direct message.
            var guildUser = context.User as IGuildUser;

            if (GuildPermission.HasValue)
            {
                if (guildUser == null) return PreconditionResult.FromError("Command must be used in a guild channel");
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value)) return PreconditionResult.FromError($"Command requires guild permission {GuildPermission.Value}");
            }

            if (ChannelPermission.HasValue)
            {
                // If guildChannel is null then the channel is a DM.
                var guildChannel = context.Channel as IGuildChannel;

                ChannelPermissions perms;
                if (guildChannel != null)
                    // Get the perms for the channel the command was used in if it's a guild channel.
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    // Otherwise give all permissions.
                    perms = ChannelPermissions.All(guildChannel);

                if (!perms.Has(ChannelPermission.Value))
                    return PreconditionResult.FromError($"Command requires channel permission {ChannelPermission.Value}");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
