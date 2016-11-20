using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WycademyV2
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IDependencyMap _map;
        private Func<LogMessage, Task> _errorLog;

        public async Task Install(IDependencyMap map, Func<LogMessage, Task> log)
        {
            // Extract the client from the dependency map.
            _client = map.Get<DiscordSocketClient>();
            // Initialize the CommandService and add it to the map.
            _commands = new CommandService();
            map.Add(_commands);
            _map = map;
            _errorLog = log;

            // Add all modules in the assembly to the CommandSercice.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage msg)
        {
            // If userMessage is null, then it's a system message that should just be ignored.
            var userMessage = msg as SocketUserMessage;
            if (userMessage == null) return;

            // The character index to start parsing the command at.
            int argPos = 0;

            if (userMessage.HasCharPrefix('<', ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new CommandContext(_client, userMessage);
                var result = await _commands.ExecuteAsync(context, argPos, _map);

                if (!result.IsSuccess)
                {
                    await userMessage.Channel.SendMessageAsync(":interrobang: An error has occurred and been logged to the console. If this happens again, contact Iwuh#6351.");
                    await _errorLog(new LogMessage(LogSeverity.Error, "Command Error!", result.ErrorReason));
                }
            }
        }
    }
}
