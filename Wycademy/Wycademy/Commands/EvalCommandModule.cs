using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
using Discord.Modules;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    class EvalCommandModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        // Add references to mscorlib, system.core, and Discord.Net's assemblies. Import various essential namespaces.
        ScriptOptions evalOptions = ScriptOptions.Default
            .WithReferences(typeof(System.Object).Assembly, typeof(System.Linq.Enumerable).Assembly, typeof(Discord.Server).Assembly, typeof(Discord.Commands.CommandEventArgs).Assembly)
            .WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks", "Discord", "Discord.Commands");

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateCommand("eval")
                .MinPermissions((int)PermissionLevels.BotOwner)
                .UseGlobalBlacklist()
                .Description("Evaluates a C# expression.")
                .Parameter("Expression", ParameterType.Unparsed)
                .Do(async e =>
                {
                    try
                    {
                        // We use an instance of ScriptHost to allow the expression to access the current client and event args.
                        object result = await CSharpScript.EvaluateAsync(e.GetArg("Expression"), options: evalOptions, globals: new ScriptHost(_client, e));

                        Message m = await e.Channel.SendMessage(result.ToString());
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
                    }
                    catch (CompilationErrorException ex)
                    {
                        Message m = await e.Channel.SendMessage(ex.Message);
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
                    }
                });
            });
        }
    }

    public class ScriptHost
    {
        // Provides a class to pass as a global when evaluating expressions.
        public DiscordClient Client { get; set; }
        public CommandEventArgs Args { get; set; }

        public ScriptHost(DiscordClient c, CommandEventArgs e)
        {
            Client = c;
            Args = e;
        }
    }
}
