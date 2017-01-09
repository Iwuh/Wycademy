using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands
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
            // Set the method for error logging.
            _errorLog = log;
            // Add services to the dependency map.
            await AddServices(_map);

            // Add all modules in the assembly to the CommandService.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage msg)
        {
            // If userMessage is null, then it's a system message that should just be ignored.
            var userMessage = msg as SocketUserMessage;
            if (userMessage == null) return;
            // Ignore any bot messages.
            if (userMessage.Author.IsBot) return;
            // Ignore any users on the blacklist.
            if (_map.Get<BlacklistService>().CheckBlacklist(msg.Author.Id, BlacklistType.User)) return;

            // The character index to start parsing the command at.
            int argPos = 0;
#if BETA
            if (userMessage.HasStringPrefix("<<", ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
#else
            if (userMessage.HasCharPrefix('<', ref argPos) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
#endif
            {
                var context = new CommandContext(_client, userMessage);
                var result = await _commands.ExecuteAsync(context, argPos, _map);

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            await msg.Channel.SendMessageAsync("Error: Invalid argument count. Try `<help [command]`.");
                            break;
                        case CommandError.Exception:
                            if (msg.Author.Id == (await _client.GetApplicationInfoAsync()).Owner.Id)
                            {
                                // If the command was called by the owner show the full exception message.
                                await msg.Channel.SendMessageAsync("Exception: " + result.ErrorReason);
                            }
                            else
                            {
                                // Otherwise show a generic message and log to the console.
                                await msg.Channel.SendMessageAsync(":interrobang: An exception occured and has been logged to the console. If this happens again, contact Iwuh#6351.");
                                await _errorLog(new LogMessage(LogSeverity.Error, "Command Error", result.ErrorReason));
                            }
                            break;
                        case CommandError.ParseFailed:
                            await msg.Channel.SendMessageAsync("Error: Invalid input. Double check your quotation marks and numbers.");
                            break;
                        case CommandError.UnmetPrecondition:
                            await msg.Channel.SendMessageAsync("A requirement to execute this command was not met: " + result.ErrorReason);
                            break;
                    }
                }
            }
        }

        private Task AddServices(IDependencyMap map)
        {
            map.Add(new MonsterInfoService());
            map.Add(new LockerService());
            map.Add(new MotionValueService());
            map.Add(new CommandCacheService(map, 500));
            map.Add(new RandomNumberService());
            map.Add(new DamageCalculatorService(map.Get<DiscordSocketClient>()));
            return Task.CompletedTask;
        }
    }
}
