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
    public class HelpModule : ModuleBase
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

        //[Command("help")]
        //[Summary("What did you expect to find here?")]
        //[RequireUnlocked]
        //public async Task GetGeneralHelp()
        //{
        //    // NB: I put "group" in the remarks if the module uses GroupAttribute.

        //    var helpBuilder = new StringBuilder();
        //    // Used to prevent a command from being listed twice.
        //    var listedCommands = new List<CommandInfo>();
        //    foreach (ModuleInfo module in _commands.Modules)
        //    {
        //        // Skip the module if none of the commands are usable by the user in the current context.
        //        if (module.Commands.None(c =>
        //        {
        //            var result = c.CheckPreconditionsAsync(Context, _provider).Result;
        //            return result.IsSuccess;
        //        })) continue;

        //        // Skip the module if it's marked hidden.
        //        if (module.Remarks != null && module.Remarks.Contains("hidden")) continue;

        //        foreach (ModuleInfo submodule in module.Submodules)
        //        {
        //            foreach (CommandInfo command in submodule.Commands)
        //            {
        //                // If the module uses a special group name, add it.
        //                if (module.Remarks != null && module.Remarks.Contains("group"))
        //                {
        //                    helpBuilder.Append($"`{module.Name}` ");
        //                }

        //                // If the submodule uses a special group name, add it.
        //                if (submodule.Remarks != null && submodule.Remarks.Contains("group"))
        //                {
        //                    helpBuilder.Append($"`{submodule.Name}` ");
        //                }

        //                // Add the command name and its summary.
        //                helpBuilder.AppendLine($"`{command.Name}` - {command.Summary ?? "There is no summary for this command."}");
        //                helpBuilder.AppendLine();

        //                // Add the command to a list of used commands so that the same command isn't added twice.
        //                listedCommands.Add(command);
        //            }
        //        }

        //        foreach (CommandInfo command in module.Commands)
        //        {
        //            // Skip over any commands that have already been processed in the submodule check.
        //            if (listedCommands.Contains(command)) continue;

        //            // If the module uses a special name, add it.
        //            if (module.Remarks != null && module.Remarks.Contains("group"))
        //            {
        //                helpBuilder.Append($"`{module.Name}` ");
        //            }

        //            helpBuilder.AppendLine($"`{command.Name}` - {command.Summary ?? "There is no summary for this command."}");
        //            helpBuilder.AppendLine();
        //        }
        //    }

        //    helpBuilder.AppendLine("To see help for an individual command, do `<help [command]` where `[command]` is the command you want info about. ex. `<help hitzone`");
        //    helpBuilder.AppendLine("For support, feature requests, and bug reports, please join the development server: https://discord.gg/R8g3BCS");

        //    var dm = await Context.User.GetDMChannelAsync() ?? await Context.User.CreateDMChannelAsync();
        //    await dm.SendMessageAsync(helpBuilder.ToString());
        //    await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));
        //}
        
        [Command("help")]
        [Summary("Why are you even looking at this?")]
        [RequireUnlocked]
        public async Task GetGeneralHelp()
        {
            var helpBuilder = new StringBuilder();

            foreach (CommandInfo command in _commands.Commands)
            {
                if (GetCommandUsage(command, out string usage))
                {
                    // Add the summary, or an error message if there is no summary.
                    helpBuilder.AppendLine($"`{usage}` - {command.Summary ?? "There is no summary for this command."}");
                    helpBuilder.AppendLine();
                }
            }

            helpBuilder.AppendLine("To see help for an individual command, do `<help [command]` where `[command]` is the command you want info about. ex. `<help hitzone`");
            helpBuilder.AppendLine("For support, feature requests, and bug reports, please join the development server: https://discord.gg/R8g3BCS");

            var dm = await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync(helpBuilder.ToString());
            await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));

            bool GetCommandUsage(CommandInfo cmd, out string usage)
            {
                string fullPath = cmd.Name;
                bool show = true;

                ModuleInfo currentModule = cmd.Module;
                while (currentModule.Remarks != null && currentModule.Remarks.Contains("group"))
                {
                    // NB: I put "group" in the remarks if the module uses GroupAttribute, as Discord.Net does not provide a way to check.
                    // NB: "hidden" in a module's remarks indicates that its commands should not be shown in help.

                    if (currentModule.Remarks.Contains("hidden"))
                    {
                        // If the module should be hidden, set show to false and stop looping.
                        show = false;
                        break;
                    }

                    // Add the module name to the front of the command.
                    fullPath = fullPath.Insert(0, $"{currentModule.Name} ");
                    if (currentModule.IsSubmodule)
                    {
                        // Go another step up if applicable.
                        currentModule = currentModule.Parent;
                    }
                    else
                    {
                        // Otherwise, stop looping.
                        break;
                    }
                }
                // Add the bot prefix to the front of the command usage.
                fullPath = fullPath.Insert(0, "<");

                usage = fullPath;
                return show;
            }
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

                var dm = await Context.User.CreateDMChannelAsync();
                await dm.SendMessageAsync(helpBuilder.ToString());
                await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));
            }
        }
    }
}