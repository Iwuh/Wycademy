using KiranicoScraper.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KiranicoScraper
{
    public static class KiranicoScraper
    {
        /// <summary>
        /// Executes all scrapers, populating the database.
        /// </summary>
        /// <param name="logger"></param>
        public static void RunScrapers(ILoggerFactory loggerFactory, IConfiguration config)
        {
            // We can't use the generic overload of CreateLogger because this class is static.
            var logger = loggerFactory.CreateLogger("KiranicoScraper.KiranicoScraper");

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

            using (var database = new DbManager(loggerFactory.CreateLogger<DbManager>(), config))
            using (var requester = new WebRequester(loggerFactory.CreateLogger<WebRequester>()))
            {
                foreach (var scraper in scrapers)
                {
                    logger.LogInformation($"Starting scraper {scraper.GetType()}.");

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
                        logger.LogError(ex, $"An exception was thrown during execution of scraper {scraper.GetType()}.");
                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }

            stopwatch.Stop();
            logger.LogInformation($"All scrapers finished running in {stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s.");
        }
    }
}
