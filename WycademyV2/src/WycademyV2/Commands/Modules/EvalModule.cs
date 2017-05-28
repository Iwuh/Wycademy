using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class EvalModule : ModuleBase
    {
        private CommandCacheService _cache;
        private EvalService _eval;
        private IServiceProvider _provider;

        public EvalModule(CommandCacheService cache, EvalService eval, IServiceProvider provider)
        {
            _cache = cache;
            _eval = eval;
            _provider = provider;
        }

        [Command("eval")]
        [Summary("Evaluates a C# expression using the Roslyn scripting API.")]
        [RequireOwner]
        public async Task EvaluateExpression([Remainder, Summary("The C# expression to evaluate.")] string expr)
        {
            var result = await _eval.EvaluateAsync(expr, Context, _provider);

            string message;
            if (!result.IsSuccess)
            {
                message = $"Compilation Failed:\n```{result.Output}```";
            }
            else
            {
                message = $"Evaluated Successfully:\n```{result.Output}```";
            }

            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: message, prependZWSP: true);
        }
    }
}
