using CommandLine;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using KiranicoScraper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Npgsql;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wycademy.Commands;
using Wycademy.Commands.Enums;
using Wycademy.Commands.Models;
using Wycademy.Commands.Services;
using Wycademy.Core.Models;

namespace Wycademy
{
    public class Program
    {
        // Launch the async main method and wait until it returns.
        public static void Main(string[] args)
        {
            bool shouldRebuild = args.Length > 0 && args[0] == "rebuild";
            new Program().Start(shouldRebuild).GetAwaiter().GetResult();
        }

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private IServiceProvider _provider;
        private IConfiguration _config;
        private ILogger _logger;

        private async Task Start(bool shouldRebuildDatabase)
        {
            // Load the configuration file.
            _config = new ConfigurationBuilder()
                .AddIniFile("WycademyConfig.ini", optional: false, reloadOnChange: false)
                .Build();

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
            token = _config["Discord:Token"];
#endif

            // Build the IServiceProvider that will be used for DI across all Wycademy projects.
            _provider = await ConfigureServices();
            _logger = _provider.GetService<ILogger<Program>>();

            // Initialize the CommandHandler.
            _handler = new CommandHandler();
            await _handler.Install(_provider);

            var scraper = new ScraperManager(_provider);
            scraper.RunScrapers();

            // Log in and connect.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

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

            // Add logging.
            services.AddLogging(builder => builder.AddNLog());

            // Set up Entity Framework Core with Npgsql.
            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<MonsterContext>(ServiceLifetime.Transient); // This will be removed when the database conversion is finished
            services.AddDbContext<WycademyContext>(options =>
            {
                var connectionString = new NpgsqlConnectionStringBuilder()
                {
                    Host = "localhost",
                    Port = int.Parse(_config["Database:Port"]),
                    Database = _config["Database:Name"],
                    Username = _config["Database:User"],
                    Passfile = _config["Database:Passfile"]
                };
                options.UseNpgsql(connectionString.ToString());
            }, ServiceLifetime.Scoped);

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
            _logger.LogDiscord(msg);

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
