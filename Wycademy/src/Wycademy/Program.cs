﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wycademy.Commands;
using Wycademy.Commands.Enums;
using Wycademy.Commands.Models;
using Wycademy.Commands.Services;

namespace Wycademy
{
    public class Program
    {
        // Convert synchronous static Main to an async non-static main method.
        public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private IServiceProvider _provider;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task Start()
        {
            // Initialize the DiscordSocketClient and set the LogLevel.
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
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
                // Ignore the message if it's by a bot.
                if (userMessage.Author.IsBot) return;

                // To save on processing, only check if the bot was mentioned if there is at least one mention.
                if (userMessage.MentionedUsers.Count > 0)
                {
                    // If the message is mentioning the bot...
                    if (userMessage.MentionedUsers.Select(x => x.Id).Contains(_client.CurrentUser.Id))
                    {
                        // Make sure the bot has permission to add reactions if this is a guild context.
                        if (userMessage.Channel is SocketGuildChannel channel)
                        {
                            var perms = channel.Guild.CurrentUser.GetPermissions(channel);
                            if (!perms.AddReactions) return;
                        }
                        // React with eyes.
                        await userMessage.AddReactionAsync(new Emoji("👀"));
                    }
                }
            };
            _client.JoinedGuild += async guild =>
            {
                BlacklistService blacklist = _provider.GetService<BlacklistService>();

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

            // Set token to either that of Wycademy or Wycademy Beta depending on whether the build configuration is debug or release.
            string token;
#if DEBUG
            token = Environment.GetEnvironmentVariable("WYCADEMY_BETA_TOKEN");
#else
            token = Environment.GetEnvironmentVariable("WYCADEMY_TOKEN");
#endif

            // Log in and connect.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Add services and the client to the IServiceProvider.
            _provider = await ConfigureServices();

            // Initialize the CommandHandler.
            _handler = new CommandHandler();
            await _handler.Install(_provider, Log);

            // Asynchronously block until the bot is exited.
            await Task.Delay(-1);
        }

        private async Task<IServiceProvider> ConfigureServices()
        {
            // Add all services with parameterless constructors.
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new CommandService()) // TODO: Make DefaultRunMode async
                .AddSingleton<MonsterInfoService>()
                .AddSingleton<LockerService>()
                .AddSingleton<MotionValueService>()
                .AddSingleton<UtilityService>()
                .AddSingleton<WeaponInfoService>()
                .AddSingleton<EvalService>();

            // Add all services with custom constructors.
            services.AddSingleton(new CommandCacheService(_client, 500))
                .AddSingleton(new DamageCalculatorService(_client))
                .AddSingleton(new ReactionMenuService(_client));

            // Add the blacklist service.
            var blacklist = new BlacklistService();
            await blacklist.LoadAsync();
            services.AddSingleton(blacklist);

            // Add the monster DB context.
            services.AddDbContext<MonsterContext>(ServiceLifetime.Transient);

            // Build the collection into an IServiceProvider.
            var provider = services.BuildServiceProvider();

            // Request certain services to create them (a singleton is not created until the first time it is requested).
            provider.GetService<UtilityService>();
            provider.GetService<WeaponInfoService>();

            // Return the provider.
            return provider;
        }

        private Task Log(LogMessage msg)
        {
            // Set the function to use for logging.
            Action<Exception, string> logFunc;
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    logFunc = _logger.Fatal;
                    break;
                case LogSeverity.Error:
                    logFunc = _logger.Error;
                    break;
                case LogSeverity.Warning:
                    logFunc = _logger.Warn;
                    break;
                case LogSeverity.Info:
                    logFunc = _logger.Info;
                    break;
                case LogSeverity.Verbose:
                    logFunc = _logger.Debug;
                    break;
                case LogSeverity.Debug:
                    logFunc = _logger.Trace;
                    break;
                default:
                    // This should never be reached, but if it is, use an empty action.
                    logFunc = (ex, str) => { };
                    break;
            }

            // Call the function. Omit the timestamp from the LogMessage because a timestamp is already included by NLog.
            logFunc(msg.Exception, msg.ToString(prependTimestamp: false));

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
