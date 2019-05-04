using KiranicoScraper.Scrapers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wycademy.Core.Models;

namespace KiranicoScraper
{
    public class ScraperManager
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ScraperManager> _logger;

        /// <summary>
        /// Creates a new <see cref="ScraperManager"/> instance.
        /// </summary>
        /// <param name="provider">An <see cref="IServiceProvider"/> used to retrieve logging and Entity Framework services.</param>
        public ScraperManager(IServiceProvider provider)
        {
            _provider = provider;
            _logger = provider.GetRequiredService<ILogger<ScraperManager>>();
        }

        /// <summary>
        /// Executes all scrapers, populating the database.
        /// </summary>
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

            // Ensure the database schema is created and fully updated.
            using (var context = _provider.GetRequiredService<WycademyContext>())
            {
                context.Database.Migrate();
            }
            using (var requester = new WebRequester(_provider))
            {
                foreach (var scraper in scrapers)
                {
                    _logger.LogInformation($"Starting scraper {scraper.GetType()}.");

                    try
                    {
                        scraper.Requester = requester;
                        scraper.Execute();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An exception was thrown during execution of scraper {scraper.GetType()}.");
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
    }
}
