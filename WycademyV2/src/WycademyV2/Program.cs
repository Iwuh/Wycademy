using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Services;

namespace WycademyV2
{
    public class Program
    {
        // Convert synchronous static Main to an async non-static main method.
        public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private DependencyMap _map;

        public async Task Start()
        {
            // Initialize the DiscordSocketClient and set the LogLevel.
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 1000
            });

            // Hook up a method that prints all logs to the console.
            _client.Log += Log;

            _client.MessageReceived += async message =>
            {
                // Log any recieved messages to the console.
                await Log(new LogMessage(LogSeverity.Info, message.Author.Id == _client.CurrentUser.Id ? ">>Message" : "<<Message", GetUserLogMessage(message)));
            };
            _client.MessageReceived += async message =>
            {
                // If userMessage is null then it's a system message, which should be ignored.
                var userMessage = message as SocketUserMessage;
                if (userMessage == null) return;

                // To save on processing, only check if the bot was mentioned if there is at least one mention.
                if (userMessage.MentionedUsers.Count > 0)
                {
                    // If the message is mentioning the bot...
                    if (userMessage.MentionedUsers.Select(x => x.Id).Contains(_client.CurrentUser.Id))
                    {
                        // React with eyes.
                        await userMessage.AddReactionAsync("👀");
                    }
                }
            };
            _client.JoinedGuild += async guild =>
            {
                BlacklistService blacklist = _map.Get<BlacklistService>();

                if (blacklist.CheckBlacklist(guild.Id, BlacklistType.Guild))
                {
                    var channel = guild.DefaultChannel;
                    await channel.SendMessageAsync("This guild has been blacklisted. Wycademy will now leave.");
                    await Task.Delay(5000);
                    await guild.LeaveAsync();
                }
                else if (blacklist.CheckBlacklist(guild.OwnerId, BlacklistType.GuildOwner))
                {
                    var channel = guild.DefaultChannel;
                    await channel.SendMessageAsync($"This guild's owner has been blacklisted. Wycademy will now leave, and cannot be added to any other guilds owned by {guild.Owner}.");
                    await Task.Delay(5000);
                    await guild.LeaveAsync();
                }
            };

            // Set token to either that of Wycademy or Wycademy Beta depending on whether or not the BETA flag is defined in the build options.
            string token;
#if BETA
            token = Environment.GetEnvironmentVariable("WYCADEMY_BETA_TOKEN");
#else
            token = Environment.GetEnvironmentVariable("WYCADEMY_TOKEN");
#endif

            // Log in and connect.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.ConnectAsync();

            // Add the client to the DependencyMap that will be used during command execution.
            _map = new DependencyMap();
            _map.Add(_client);

            // Initialize and add the CommandHandler to the map.
            _handler = new CommandHandler();
            await _handler.Install(_map, Log);

            // Asynchronously block until the bot is exited.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.WriteLine(msg.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            // Represents a completed Task for methods that have to return Task but don't do any asynchronous work.
            return Task.CompletedTask;
        }

        private string GetUserLogMessage(SocketMessage msg)
        {
            // If channel is null, then it's a DM message. Otherwise it's a Guild message.
            var channel = msg.Channel as SocketGuildChannel;

            bool isPrivate = channel == null;

            return $"{(isPrivate ? "Private" : channel.Guild.Name)}{(isPrivate ? "" : "/#" + channel.Name)} from {msg.Author}: {msg.Content}";
        }
    }
}
