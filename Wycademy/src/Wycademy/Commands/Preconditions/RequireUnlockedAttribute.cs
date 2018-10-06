using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Preconditions
{
    public class RequireUnlockedAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var locker = provider.GetService<LockerService>();

            if (!locker.IsLocked)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Commands are currently locked."));
        }
    }
}
