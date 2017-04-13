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
            // NB: I put "group" in the remarks if the module uses GroupAttribute.

            var helpBuilder = new StringBuilder();
            // Used to prevent a command from being listed twice.
            var listedCommands = new List<CommandInfo>();
            foreach (ModuleInfo module in _commands.Modules)
            {
                // Skip the module if none of the commands are usable by the user in the current context.
                if (module.Commands.None(c =>
                {
                    var result = c.CheckPreconditionsAsync(Context, _map).Result;
                    return result.IsSuccess;
                })) continue;

                // Skip the module if it's marked hidden.
                if (module.Remarks != null && module.Remarks.Contains("hidden")) continue;

                foreach (ModuleInfo submodule in module.Submodules)
                {
                    foreach (CommandInfo command in submodule.Commands)
                    {
                        // If the module uses a special group name, add it.
                        if (module.Remarks != null && module.Remarks.Contains("group"))
                        {
                            helpBuilder.Append($"`{module.Name}` ");
                        }

                        // If the submodule uses a special group name, add it.
                        if (submodule.Remarks != null && submodule.Remarks.Contains("group"))
                        {
                            helpBuilder.Append($"`{submodule.Name}` ");
                        }

                        // Add the command name and its summary.
                        helpBuilder.AppendLine($"`{command.Name}` - {command.Summary ?? "There is no summary for this command."}");
                        helpBuilder.AppendLine();

                        // Add the command to a list of used commands so that the same command isn't added twice.
                        listedCommands.Add(command);
                    }
                }

                foreach (CommandInfo command in module.Commands)
                {
                    // Skip over any commands that have already been processed in the submodule check.
                    if (listedCommands.Contains(command)) continue;

                    // If the module uses a special name, add it.
                    if (module.Remarks != null && module.Remarks.Contains("group"))
                    {
                        helpBuilder.Append($"`{module.Name}` ");
                    }

                    helpBuilder.AppendLine($"`{command.Name}` - {command.Summary ?? "There is no summary for this command."}");
                    helpBuilder.AppendLine();
                }
            }

            helpBuilder.AppendLine("To see help for an individual command, do `<help [command]` where `[command]` is the command you want info about. ex. `<help hitzone`");

            var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync(helpBuilder.ToString());
            await Context.Message.AddReactionAsync(WycademyConst.HELP_REACTION);
        }

        [Command("help")]
        [Summary("...")]
        [RequireUnlocked]
        public async Task GetCommandHelp([Remainder, Summary("The command to search for.")] string query)
        {
            var result = _commands.Search(Context, query);

            if (result.Commands != null)
            {
                var helpBuilder = new StringBuilder();

                foreach (CommandInfo command in result.Commands.Select(x => x.Command))
                {
                    helpBuilder.AppendLine(Format.Underline(command.Module.Summary));
                    helpBuilder.AppendLine($"{Format.Bold(command.Name)} ({(command.Summary ?? "There is no summary available for this command.")})");
                    helpBuilder.AppendLine();
                    if (command.Aliases.Count >= 1)
                    {
                        helpBuilder.AppendLine(Format.Italics($"Aliases: {string.Join(" ", command.Aliases.Where(a => a != command.Name))}"));
                    }
                    helpBuilder.AppendLine();

                    foreach (ParameterInfo parameter in command.Parameters)
                    {
                        helpBuilder.AppendLine($"{(parameter.IsOptional ? "(Optional) " : "")}`{parameter.Name}` - {(parameter.Summary ?? "There is no summary available for this parameter.")}");
                        helpBuilder.AppendLine();
                    }

                    helpBuilder.AppendLine();
                }

                var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
                await dm.SendCachedMessageAsync(Context.Message.Id, _cache, text: helpBuilder.ToString(), prependZWSP: true);
                await Context.Message.AddReactionAsync(WycademyConst.HELP_REACTION);
            }
        }
    }
}
