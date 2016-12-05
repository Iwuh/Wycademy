using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [Remarks("hidden")]
    public class HelpModule : ModuleBase
    {
        private CommandService _commands;
        private CommandCacheService _cache;
        private IDependencyMap _map;

        public HelpModule(CommandService cmds, CommandCacheService ccs, IDependencyMap map)
        {
            _commands = cmds;
            _cache = ccs;
            _map = map;
        }

        [Command("help")]
        [Summary("What did you expect to find here?")]
        [RequireUnlocked]
        public async Task GetGeneralHelp()
        {
            var helpMessages = new List<string>();
            foreach (var module in _commands.Modules)
            {
                // Used to not show the category if none of the commands are available to the user.
                bool anyCommandsMatched = false;

                // Skip over any modules marked hidden.
                if (module.Remarks != null)
                {
                    if (module.Remarks.Contains("hidden")) continue;
                }

                var moduleBuilder = new StringBuilder();
                moduleBuilder.AppendLine($"{module.Summary}:");
                foreach (var command in module.Commands)
                {
                    // Check the preconditions to make sure that the user is allowed to use this command.
                    var result = await command.CheckPreconditionsAsync(Context, _map);

                    // Move to the next command if it didn't pass the checks.
                    if (!result.IsSuccess) continue;

                    // If it gets this far, then at least one command has passed.
                    anyCommandsMatched = true;
                    moduleBuilder.AppendLine($"`{command.Name}` - {command.Summary}");
                }

                if (anyCommandsMatched)
                {
                    // Only add the help for that module if at least one command will be shown.
                    helpMessages.Add(moduleBuilder.ToString());
                }
            }

            var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
            await dm.SendCachedMessageAsync(Context.Message.Id, _cache, text: string.Join("\n", helpMessages), prependZWSP: true);
        }
    }
}
