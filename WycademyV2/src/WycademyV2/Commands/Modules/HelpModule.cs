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

            await Context.User.SendMessageAsync(helpBuilder.ToString());
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

                await Context.User.SendMessageAsync(helpBuilder.ToString());
                await Context.Message.AddReactionAsync(new Emoji(WycademyConst.HELP_REACTION));
            }
        }
    }
}
