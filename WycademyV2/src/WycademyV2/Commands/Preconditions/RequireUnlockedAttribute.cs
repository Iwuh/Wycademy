using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Preconditions
{
    public class RequireUnlockedAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command, IDependencyMap map)
        {
            var locker = map.Get<LockerService>();

            if (!locker.IsLocked)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Commands are currently locked."));
        }
    }
}
