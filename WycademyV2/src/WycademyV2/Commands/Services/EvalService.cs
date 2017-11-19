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
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Services
{
    public class EvalService
    {
        //private ScriptOptions _evalOptions;
        //private DiscordSocketClient _client;

        //public EvalService(DiscordSocketClient client)
        //{
        //    _client = client;

        //    // Create script options with references to the core library assemblies, as well as certain Discord.Net assemblies.
        //    // Also add imports for various essential namespaces.
        //    _evalOptions = ScriptOptions.Default
        //    .WithReferences(typeof(object).GetTypeInfo().Assembly, typeof(Enumerable).GetTypeInfo().Assembly, typeof(Embed).GetTypeInfo().Assembly,
        //        typeof(CommandContext).GetTypeInfo().Assembly, typeof(DiscordSocketClient).GetTypeInfo().Assembly, Assembly.GetEntryAssembly())
        //    .WithImports("System", "System.Linq", "System.Threading.Tasks", "System.Collections.Generic", "System.Text", "System.IO", "Discord", "Discord.Commands", "Discord.WebSocket",
        //                 "WycademyV2", "WycademyV2.Commands.Services", "Microsoft.Extensions.DependencyInjection");
        //}

        //public async Task<EvalResult> EvaluateAsync(string input, SocketCommandContext context, IServiceProvider provider)
        //{
        //    bool successful;
        //    string output;

        //    try
        //    {
        //        // Evaluate the input, using the current client and context as global variables.
        //        object result = await CSharpScript.EvaluateAsync(input, options: _evalOptions, globals: new ScriptHost(context.Client, context, provider));

        //        successful = true;
        //        output = result?.ToString() ?? "null";
        //    }
        //    catch (CompilationErrorException e)
        //    {
        //        successful = false;
        //        output = e.Message;
        //    }

        //    return new EvalResult(successful, output);
        //}

        //public class ScriptHost
        //{
        //    public DiscordSocketClient Client { get; }
        //    public SocketCommandContext Context { get; }
        //    public IServiceProvider Provider { get; }

        //    public ScriptHost(DiscordSocketClient client, SocketCommandContext context, IServiceProvider provider)
        //    {
        //        Client = client;
        //        Context = context;
        //        Provider = provider;
        //    }
        //}

        private ScriptOptions _options;
        private EvalEnvironment _environment;

        public EvalService()
        {
            _environment = new EvalEnvironment();

            // Create the ScriptOptions used for evaluating with some essential assemblies and namespaces.
            _options = ScriptOptions.Default
            .WithReferences(typeof(object).GetTypeInfo().Assembly, typeof(Enumerable).GetTypeInfo().Assembly, typeof(Embed).GetTypeInfo().Assembly,
                typeof(CommandContext).GetTypeInfo().Assembly, typeof(DiscordSocketClient).GetTypeInfo().Assembly, Assembly.GetEntryAssembly())
            .WithImports("System", "System.Linq", "System.Threading.Tasks", "System.Collections.Generic", "System.Text", "System.IO", "Discord", "Discord.Commands", "Discord.WebSocket",
                         "WycademyV2", "WycademyV2.Commands.Services", "Microsoft.Extensions.DependencyInjection");
        }

        public async Task<EvalResult> EvaluateAsync(string expression, SocketCommandContext context, IServiceProvider provider)
        {
            bool successful;
            string output;
            Exception exception;

            try
            {
                object result = await CSharpScript.EvaluateAsync(expression, _options, new EvalGlobals(context, provider, _environment));

                successful = true;
                output = result?.ToString() ?? "null";
                exception = null;
            }
            catch (CompilationErrorException ex)
            {
                successful = false;
                output = $"An error occurred during compilation";
                exception = ex;
            }
            catch (Exception ex)
            {
                successful = false;
                output = $"An error occurred during execution";
                exception = ex;
            }

            return new EvalResult(successful, output, exception);
        }
    }
}
