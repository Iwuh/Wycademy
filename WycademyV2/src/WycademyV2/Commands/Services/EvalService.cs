using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Services
{
    public class EvalService
    {
        private ScriptOptions _evalOptions;
        private DiscordSocketClient _client;

        public EvalService(DiscordSocketClient client)
        {
            _client = client;

            // Create script options with references to the core library assemblies, as well as certain Discord.Net assemblies.
            // Also add imports for various essential namespaces.
            _evalOptions = ScriptOptions.Default
            .WithReferences(typeof(object).GetTypeInfo().Assembly, typeof(Enumerable).GetTypeInfo().Assembly, typeof(Embed).GetTypeInfo().Assembly,
                typeof(CommandContext).GetTypeInfo().Assembly, typeof(DiscordSocketClient).GetTypeInfo().Assembly)
            .WithImports("System", "System.Linq", "System.Threading.Tasks", "System.Collections.Generic", "System.Text", "System.IO", "Discord", "Discord.Commands", "Discord.WebSocket");
        }

        public async Task<EvalResult> EvaluateAsync(string input, DiscordSocketClient client, CommandContext context)
        {
            bool successful;
            string output;

            try
            {
                // Evaluate the input, using the current client and context as global variables.
                object result = await CSharpScript.EvaluateAsync(input, options: _evalOptions, globals: new ScriptHost(client, context));

                successful = true;
                output = result?.ToString() ?? "null";
            }
            catch (CompilationErrorException e)
            {
                successful = false;
                output = e.Message;
            }

            return new EvalResult(successful, output);
        }

        public class ScriptHost
        {
            public DiscordSocketClient Client { get; set; }
            public CommandContext Context { get; set; }

            public ScriptHost(DiscordSocketClient client, CommandContext context)
            {
                Client = client;
                Context = context;
            }
        }
    }
}
