using KiranicoScraper.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KiranicoScraper
{
    public class ScraperManager
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ScraperManager> _logger;
        private readonly IConfiguration _config;

        /// <summary>
        /// Creates a new <see cref="ScraperManager"/> instance.
        /// </summary>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory"/> used to create a logger for this class.</param>
        /// <param name="config">The bot configs used to access the database.</param>
        public ScraperManager(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ScraperManager>();
            _config = config;
        }

        /// <summary>
        /// Executes all scrapers, populating the database.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public void RunScrapers()
        {
            var scrapers = new List<Scraper>()
            {
                new MonsterScraper4U(),
                new MonsterScraperGen(),
                new MonsterScraperWorld(),
                new WeaponScraper4U(),
                new WeaponScraperGen()
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var database = new DbManager(_loggerFactory.CreateLogger<DbManager>(), _config))
            using (var requester = new WebRequester(_loggerFactory.CreateLogger<WebRequester>()))
            {
                foreach (var scraper in scrapers)
                {
                    _logger.LogInformation($"Starting scraper {scraper.GetType()}.");

                    NpgsqlTransaction transaction = null;
                    try
                    {
                        scraper.Database = database;
                        scraper.Requester = requester;

                        transaction = database.BeginTransaction();
                        scraper.Execute();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An exception was thrown during execution of scraper {scraper.GetType()}.");
                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
            // ImageSharp keeps ArrayPools for performance reasons. However, this results in a high memory footprint and as we only use ImageSharp in the scraper, we
            // release these retained resources.
            SixLabors.ImageSharp.Configuration.Default.MemoryAllocator.ReleaseRetainedResources();

            stopwatch.Stop();
            _logger.LogInformation($"All scrapers finished running in {stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s.");
        }

        public void RecreateDatabase()
        {
            // Determine if the platform is Windows or Linux.
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _logger.LogDebug($"Determined OS to be {(isWindows ? "Windows" : "Linux")}.");

            // Create a Process to drop & recreate the database using either CMD or bash, depending on the platform.
            var terminalProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = isWindows ? "cmd.exe" : "/bin/bash",
                    Arguments = isWindows ? $"/c \"{GetDropdbCommand()} & {GetCreatedbCommand()}\"" : $"-c \"{GetDropdbCommand()} ; {GetCreatedbCommand()}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            // Run the process and then wait up to 20 seconds for it to finish.
            _logger.LogInformation($"Dropping and recreating database '{_config["Database:Name"]}'.");
            terminalProcess.Start();
            terminalProcess.WaitForExit(20000);
        }

        private string GetDropdbCommand() => $"dropdb -h localhost -p {_config["Database:Port"]} -U {_config["Database:User"]} -w {_config["Database:Name"]}";
        private string GetCreatedbCommand() => $"createdb -h localhost -p {_config["Database:Port"]} -U {_config["Database:User"]} -w {_config["Database:Name"]}";
    }
}
