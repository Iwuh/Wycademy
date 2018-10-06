using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class EvalGlobals
    {
        public DiscordSocketClient Client { get; private set; }
        public SocketCommandContext Context { get; private set; }
        public IServiceProvider Provider { get; private set; }
        public EvalEnvironment Environment { get; private set; }

        public EvalGlobals(SocketCommandContext context, IServiceProvider provider, EvalEnvironment environment)
        {
            Client = context.Client;
            Context = context;
            Provider = provider;
            Environment = environment;
        }
    }
}
