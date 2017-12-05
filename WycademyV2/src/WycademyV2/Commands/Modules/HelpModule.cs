﻿using Discord;
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
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _commands;
        private CommandCacheService _cache;
        private IServiceProvider _provider;

        public HelpModule(CommandService cmds, CommandCacheService ccs, IServiceProvider provider)
        {
            _commands = cmds;
            _cache = ccs;
            _provider = provider;
        }
        
        [Command("help", RunMode = RunMode.Async)]
        [Summary("Why are you even looking at this?")]
        [RequireUnlocked]
        public async Task GetGeneralHelp()
        {
            var helpBuilder = new StringBuilder();

            foreach (var module in _commands.Modules)
            {
                // The below code throws an ArgumentException in a DM with Discord.Net v1.0.2. The issue has been fixed, and the code will be uncommented
                // once the changes are pushed to stable.

                //bool usable = false;
                //foreach (var command in module.Commands)
                //{
                //    var result = await command.CheckPreconditionsAsync(Context, _provider);
                //    if (result.IsSuccess)
                //    {
                //        usable = true;
                //        break;
                //    }
                //}
                //if (!usable) continue;

                // Skip over any hidden modules.
                if (module.Remarks != null && module.Remarks.Contains("hidden")) continue;

                // Check if the module is a group.
                bool isGroup = module.Aliases.First() != string.Empty;
                // Add all of the module's commands.
                helpBuilder.AppendLine($"{module.Summary}:");
                foreach (var command in module.Commands)
                {
                    string usage;
                    if (isGroup)
                    {
                        usage = $"{module.Name} {command.Name}";
                    }
                    else
                    {
                        usage = command.Name;
                    }
                    helpBuilder.AppendLine($"\t`<{usage}` - {command.Summary}");
                }

                helpBuilder.AppendLine();
            }

            helpBuilder.AppendLine("To see help for an individual command, do `<help <command>`.");
            helpBuilder.AppendLine("For support, feature requests, and bug reports, please join the development server: https://discord.gg/R8g3BCS");

            await Context.User.SendMessageAsync(helpBuilder.ToString());
            await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));
        }

        [Command("help")]
        [Summary("...")]
        [RequireUnlocked]
        public async Task GetCommandHelp([Remainder, Summary("The command to search for.")] string query)
        {
            // Search for commands that match the search query.
            var result = _commands.Search(Context, query);

            // If there are matches...
            if (result.Commands != null)
            {
                // Create a StringBuilder for the response message and get the CommandInfo of each CommandMatch.
                var helpBuilder = new StringBuilder();
                var commands = result.Commands.Select(x => x.Command);

                foreach (CommandInfo command in commands)
                {
                    // Add the command's name and its summary.
                    helpBuilder.AppendLine($"{Format.Bold(command.Name)} - ({(command.Summary ?? "There is no summary available for this command.")})");
                    if (command.Aliases.Count > 1)
                    {
                        // Add any aliases, if applicable.
                        helpBuilder.AppendLine(Format.Italics($"Aliases: {string.Join(" ", command.Aliases.Where(a => a != command.Name))}"));
                    }

                    // Add the command usage.
                    helpBuilder.Append($"Usage: `<{command.Name}");
                    foreach (var parameter in command.Parameters)
                    {
                        if (parameter.IsOptional)
                        {
                            helpBuilder.Append($" [{parameter.Name}]");
                        }
                        else
                        {
                            helpBuilder.Append($" <{parameter.Name}>");
                        }
                    }
                    helpBuilder.AppendLine("`");

                    foreach (ParameterInfo parameter in command.Parameters)
                    {
                        // Add any parameters, if applicable.
                        helpBuilder.AppendLine($"{(parameter.IsOptional ? "(Optional) " : "")}`{parameter.Name}` - {(parameter.Summary ?? "There is no summary available for this parameter.")}");
                    }

                    if (result.Commands.Count > 1 && commands.Last() != command)
                    {
                        // If there is more than one match and we're not currently on the last match, add a divider.
                        helpBuilder.AppendLine(new string('-', 50));
                    }
                }

                await Context.User.SendMessageAsync(helpBuilder.ToString());
                await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));
            }
        }
    }
}
